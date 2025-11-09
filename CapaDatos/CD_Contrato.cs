using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_Contrato
    {
        /// <summary>
        /// Crea el contrato, genera e inserta sus N cuotas y marca el inmueble como NO disponible.
        /// TODO: la generación del cronograma se hace fuera (Negocio) y acá solo se inserta.
        /// </summary>

        // /CapaDatos/CD_Contrato.cs  (agregar a la clase existente)
        public List<ContratoAlquiler> ListarContratos(
            int? estado = null,                 // 1=Vigente, 0=Anulado, 2=Finalizado, null=Todos
            int? inmuebleId = null,             // null = todos
            string dniInquilino = null,         // null = todos
            DateTime? desde = null,             // filtra por fecha_inicio >= desde
            DateTime? hasta = null              // filtra por fecha_fin <= hasta
        )
        {
            var lista = new List<ContratoAlquiler>();

            var sb = new StringBuilder(@"
    SELECT 
        c.contrato_id, c.fecha_inicio, c.fecha_fin, c.condiciones, c.cantidad_cuotas,
        c.precio_cuota, c.fecha_creacion, c.inmueble_id, c.dni_inquilino, c.rol_inquilino_id,
        c.tasa_mora_mensual, c.estado, c.usuario_creador
    FROM dbo.contrato_alquiler c
    ");
            var where = new List<string>();
            var cmd = new SqlCommand();

            if (estado.HasValue) { where.Add("c.estado = @estado"); cmd.Parameters.AddWithValue("@estado", estado.Value); }
            if (inmuebleId.HasValue) { where.Add("c.inmueble_id = @inmueble_id"); cmd.Parameters.AddWithValue("@inmueble_id", inmuebleId.Value); }
            if (!string.IsNullOrWhiteSpace(dniInquilino)) { where.Add("c.dni_inquilino = @dni"); cmd.Parameters.AddWithValue("@dni", dniInquilino); }
            if (desde.HasValue) { where.Add("c.fecha_inicio >= @desde"); cmd.Parameters.AddWithValue("@desde", desde.Value.Date); }
            if (hasta.HasValue) { where.Add("c.fecha_fin <= @hasta"); cmd.Parameters.AddWithValue("@hasta", hasta.Value.Date); }

            if (where.Count > 0) sb.Append("WHERE " + string.Join(" AND ", where) + " ");
            sb.Append("ORDER BY c.contrato_id DESC;");

            using (var cn = new SqlConnection(Conexion.cadena))
            {
                cmd.CommandText = sb.ToString();
                cmd.Connection = cn;
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var c = new ContratoAlquiler
                        {
                            ContratoId = Convert.ToInt32(dr["contrato_id"]),
                            FechaInicio = Convert.ToDateTime(dr["fecha_inicio"]),
                            FechaFin = Convert.ToDateTime(dr["fecha_fin"]),
                            Condiciones = dr["condiciones"].ToString(),
                            CantidadCuotas = Convert.ToInt32(dr["cantidad_cuotas"]),
                            PrecioCuota = Convert.ToDecimal(dr["precio_cuota"]),
                            FechaCreacion = Convert.ToDateTime(dr["fecha_creacion"]),
                            InmuebleId = Convert.ToInt32(dr["inmueble_id"]),
                            DniInquilino = dr["dni_inquilino"].ToString(),
                            RolInquilinoId = Convert.ToInt32(dr["rol_inquilino_id"]),
                            TasaMoraMensual = Convert.ToDecimal(dr["tasa_mora_mensual"]),
                            Estado = Convert.ToInt32(dr["estado"]),
                            UsuarioCreador = Convert.ToInt32(dr["usuario_creador"])
                        };
                        lista.Add(c);
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Lista filas listas para la grilla de contratos.
        /// Trae: id, inquilino, dirección, inmueble, precio cuota, cant cuotas,
        /// fecha inicio/fin, mora diaria (a partir de la mensual) y estado.
        /// </summary>
        public List<ContratoGridRow> ListarParaGrid(int? estado = null)
        {
            var data = new List<ContratoGridRow>();

            // Nota: si preferís la fórmula exacta para diaria: (POWER(1 + c.tasa_mora_mensual/100.0, 1.0/30) - 1) * 100
            // Aquí uso aproximación lineal mensual/30 para mostrar en la UI.
            var sql = @"
SELECT
    c.contrato_id                                    AS Id,
    (p.apellido + ', ' + p.nombre)                   AS Inquilino,
    i.direccion                                      AS Direccion,
    ISNULL(i.descripcion, '')                        AS Inmueble,
    c.precio_cuota                                   AS PrecioCuota,
    c.cantidad_cuotas                                AS CantCuotas,
    c.fecha_inicio                                   AS FechaInicio,
    c.fecha_fin                                      AS FechaFin,
    CAST((c.tasa_mora_mensual / 30.0) AS decimal(7,4)) AS MoraDiaria,  -- % diario aprox
    c.estado                                         AS Estado
FROM dbo.contrato_alquiler c
INNER JOIN dbo.inmueble i   ON i.inmueble_id = c.inmueble_id
INNER JOIN dbo.persona  p   ON p.dni = c.dni_inquilino
/**/ WHERE ( @estado IS NULL OR c.estado = @estado )
ORDER BY c.contrato_id DESC;";

            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@estado", (object)estado ?? DBNull.Value);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        data.Add(new ContratoGridRow
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Inquilino = dr["Inquilino"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            Inmueble = dr["Inmueble"].ToString(),
                            PrecioCuota = Convert.ToDecimal(dr["PrecioCuota"]),
                            CantCuotas = Convert.ToInt32(dr["CantCuotas"]),
                            FechaInicio = Convert.ToDateTime(dr["FechaInicio"]),
                            FechaFin = Convert.ToDateTime(dr["FechaFin"]),
                            MoraDiaria = Convert.ToDecimal(dr["MoraDiaria"]),
                            Estado = Convert.ToInt32(dr["Estado"])
                        });
                    }
                }
            }
            return data;
        }


        public ContratoAlquiler ObtenerDetalle(int contratoId)
        {
            const string sql = @"
SELECT 
    c.contrato_id, c.fecha_inicio, c.fecha_fin, c.condiciones, c.cantidad_cuotas,
    c.precio_cuota, c.fecha_creacion, c.inmueble_id, c.dni_inquilino, c.rol_inquilino_id,
    c.tasa_mora_mensual, c.estado, c.usuario_creador,

    i.direccion AS inmueble_direccion, i.descripcion AS inmueble_desc,
    p1.nombre AS inq_nombre, p1.apellido AS inq_apellido,

    u.usuario_id, p2.nombre AS usr_nombre, p2.apellido AS usr_apellido
FROM dbo.contrato_alquiler c
INNER JOIN dbo.inmueble i ON i.inmueble_id = c.inmueble_id
INNER JOIN dbo.persona  p1 ON p1.dni = c.dni_inquilino
INNER JOIN dbo.usuario  u  ON u.usuario_id = c.usuario_creador
INNER JOIN dbo.persona  p2 ON p2.dni = u.dni
WHERE c.contrato_id = @id;
";

            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@id", contratoId);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return null;

                    var c = new ContratoAlquiler
                    {
                        ContratoId = Convert.ToInt32(dr["contrato_id"]),
                        FechaInicio = Convert.ToDateTime(dr["fecha_inicio"]),
                        FechaFin = Convert.ToDateTime(dr["fecha_fin"]),
                        Condiciones = dr["condiciones"].ToString(),
                        CantidadCuotas = Convert.ToInt32(dr["cantidad_cuotas"]),
                        PrecioCuota = Convert.ToDecimal(dr["precio_cuota"]),
                        FechaCreacion = Convert.ToDateTime(dr["fecha_creacion"]),
                        InmuebleId = Convert.ToInt32(dr["inmueble_id"]),
                        DniInquilino = dr["dni_inquilino"].ToString(),
                        RolInquilinoId = Convert.ToInt32(dr["rol_inquilino_id"]),
                        TasaMoraMensual = Convert.ToDecimal(dr["tasa_mora_mensual"]),
                        Estado = Convert.ToInt32(dr["estado"]),
                        UsuarioCreador = Convert.ToInt32(dr["usuario_creador"]),

                        oInquilino = new Persona { Nombre = dr["inq_nombre"].ToString(), Apellido = dr["inq_apellido"].ToString() },
                        oInmueble = new Inmueble { IdInmueble = Convert.ToInt32(dr["inmueble_id"]), Direccion = dr["inmueble_direccion"].ToString(), Descripcion = dr["inmueble_desc"].ToString() },
                        oUsuarioCreadorObj = new Usuario { UsuarioId = Convert.ToInt32(dr["usuario_id"]), oPersona = new Persona { Nombre = dr["usr_nombre"].ToString(), Apellido = dr["usr_apellido"].ToString() } }
                    };

                    return c;
                }
            }
        }


        public bool RegistrarContratoYCuotas(ContratoAlquiler contrato, List<Cuota> cuotas, out int contratoId, out string mensaje)
        {
            contratoId = 0;
            mensaje = string.Empty;

            using (var cn = new SqlConnection(Conexion.cadena))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 1) Regla: un inmueble no puede tener contrato vigente duplicado
                        if (ExisteContratoVigentePorInmueble(contrato.InmuebleId, cn, tx))
                        {
                            mensaje = "El inmueble ya posee un contrato vigente.";
                            tx.Rollback();
                            return false;
                        }

                        // 2) Insert contrato
                        const string sqlContrato = @"
INSERT INTO dbo.contrato_alquiler
(
    fecha_inicio,
    fecha_fin,
    condiciones,
    cantidad_cuotas,
    precio_cuota,
    fecha_creacion,
    inmueble_id,
    dni_inquilino,
    rol_inquilino_id,
    tasa_mora_mensual,
    estado,
    usuario_creador
)
VALUES
(
    @fecha_inicio,
    @fecha_fin,
    @condiciones,
    @cantidad_cuotas,
    @precio_cuota,
    @fecha_creacion,
    @inmueble_id,
    @dni_inquilino,
    @rol_inquilino_id,
    @tasa_mora_mensual,
    @estado,
    @usuario_creador
);
SELECT CAST(SCOPE_IDENTITY() AS int);";

                        using (var cmd = new SqlCommand(sqlContrato, cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@fecha_inicio", contrato.FechaInicio);
                            cmd.Parameters.AddWithValue("@fecha_fin", contrato.FechaFin);
                            cmd.Parameters.AddWithValue("@condiciones", (object)contrato.Condiciones ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@cantidad_cuotas", contrato.CantidadCuotas);
                            cmd.Parameters.AddWithValue("@precio_cuota", contrato.PrecioCuota);
                            cmd.Parameters.AddWithValue("@fecha_creacion", contrato.FechaCreacion);
                            cmd.Parameters.AddWithValue("@inmueble_id", contrato.InmuebleId);
                            cmd.Parameters.AddWithValue("@dni_inquilino", contrato.DniInquilino);
                            cmd.Parameters.AddWithValue("@rol_inquilino_id", contrato.RolInquilinoId);
                            cmd.Parameters.AddWithValue("@tasa_mora_mensual", contrato.TasaMoraMensual);
                            cmd.Parameters.AddWithValue("@estado", contrato.Estado);              // 1 = Vigente
                            cmd.Parameters.AddWithValue("@usuario_creador", contrato.UsuarioCreador);

                            contratoId = (int)cmd.ExecuteScalar();
                        }

                        // 3) Insert cuotas (bulk simple)
                        const string sqlCuota = @"
INSERT INTO dbo.cuota
(
    contrato_id,
    nro_cuota,
    periodo,
    fecha_venc,
    importe_base,
    estado,
    tasa_mora_mensual_aplicada,
    otros_adicionales_total,
    desc_adicionales
)
VALUES
(
    @contrato_id,
    @nro_cuota,
    @periodo,
    @fecha_venc,
    @importe_base,
    @estado,
    @tasa_mora_mensual_aplicada,
    @otros_adicionales_total,
    @desc_adicionales
);";

                        using (var cmd = new SqlCommand(sqlCuota, cn, tx))
                        {
                            cmd.Parameters.Add("@contrato_id", SqlDbType.Int);
                            cmd.Parameters.Add("@nro_cuota", SqlDbType.Int);
                            cmd.Parameters.Add("@periodo", SqlDbType.Char, 6);
                            cmd.Parameters.Add("@fecha_venc", SqlDbType.Date);

                            var pImporte = cmd.Parameters.Add("@importe_base", SqlDbType.Decimal);
                            pImporte.Precision = 9; pImporte.Scale = 2;

                            cmd.Parameters.Add("@estado", SqlDbType.TinyInt);

                            var pMora = cmd.Parameters.Add("@tasa_mora_mensual_aplicada", SqlDbType.Decimal);
                            pMora.Precision = 5; pMora.Scale = 2;

                            var pOtros = cmd.Parameters.Add("@otros_adicionales_total", SqlDbType.Decimal);
                            pOtros.Precision = 9; pOtros.Scale = 2;

                            cmd.Parameters.Add("@desc_adicionales", SqlDbType.VarChar, 300);

                            foreach (var q in cuotas)
                            {
                                cmd.Parameters["@contrato_id"].Value = contratoId;         // forzamos el id recién creado
                                cmd.Parameters["@nro_cuota"].Value = q.NroCuota;
                                cmd.Parameters["@periodo"].Value = q.Periodo;
                                cmd.Parameters["@fecha_venc"].Value = q.FechaVenc.Date;
                                cmd.Parameters["@importe_base"].Value = q.ImporteBase;
                                cmd.Parameters["@estado"].Value = q.Estado;               // 0 = Pendiente
                                cmd.Parameters["@tasa_mora_mensual_aplicada"].Value = q.TasaMoraMensualAplicada;
                                cmd.Parameters["@otros_adicionales_total"].Value = q.OtrosAdicionalesTotal;
                                cmd.Parameters["@desc_adicionales"].Value =
                                    string.IsNullOrWhiteSpace(q.DescAdicionales) ? (object)DBNull.Value : q.DescAdicionales;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 4) Marcar inmueble NO disponible
                        const string sqlBloqueo = @"
UPDATE dbo.inmueble
SET disponibilidad = 0
WHERE inmueble_id = @inmueble_id;";

                        using (var cmd = new SqlCommand(sqlBloqueo, cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@inmueble_id", contrato.InmuebleId);
                            cmd.ExecuteNonQuery();
                        }

                        // 5) Commit
                        tx.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { /* ignore */ }
                        mensaje = "Error al registrar contrato: " + ex.Message;
                        contratoId = 0;
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Anula el contrato (borrado lógico: estado=0) y vuelve a marcar el inmueble como disponible (1).
        /// NO toca las cuotas (quedan como registro histórico).
        /// </summary>
        public bool AnularContrato(int contratoId, out string mensaje)
        {
            mensaje = string.Empty;

            using (var cn = new SqlConnection(Conexion.cadena))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 1) Obtener inmueble del contrato
                        int inmuebleId = 0;
                        using (var cmdGet = new SqlCommand("SELECT inmueble_id FROM dbo.contrato_alquiler WHERE contrato_id = @id;", cn, tx))
                        {
                            cmdGet.Parameters.AddWithValue("@id", contratoId);
                            var val = cmdGet.ExecuteScalar();
                            if (val == null)
                            {
                                mensaje = "Contrato no encontrado.";
                                tx.Rollback();
                                return false;
                            }
                            inmuebleId = Convert.ToInt32(val);
                        }

                        // 2) Anular contrato (estado = 0)
                        using (var cmdUp = new SqlCommand(@"
UPDATE dbo.contrato_alquiler
SET estado = 0
WHERE contrato_id = @id AND estado <> 0;", cn, tx))
                        {
                            cmdUp.Parameters.AddWithValue("@id", contratoId);
                            cmdUp.ExecuteNonQuery();
                        }

                        // 3) Devolver disponibilidad al inmueble
                        using (var cmdInm = new SqlCommand(@"
UPDATE dbo.inmueble
SET disponibilidad = 1
WHERE inmueble_id = @inmueble_id;", cn, tx))
                        {
                            cmdInm.Parameters.AddWithValue("@inmueble_id", inmuebleId);
                            cmdInm.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { /* ignore */ }
                        mensaje = "Error al anular el contrato: " + ex.Message;
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Helper: ¿existe contrato vigente (estado=1) para un inmueble?
        /// </summary>
        public bool ExisteContratoVigentePorInmueble(int inmuebleId, SqlConnection cn, SqlTransaction tx)
        {
            const string sql = @"
SELECT TOP 1 1
FROM dbo.contrato_alquiler
WHERE inmueble_id = @inmueble_id AND estado = 1;";
            using (var cmd = new SqlCommand(sql, cn, tx))
            {
                cmd.Parameters.AddWithValue("@inmueble_id", inmuebleId);
                var r = cmd.ExecuteScalar();
                return r != null;
            }
        }

        /// <summary>
        /// Cambiar disponibilidad del inmueble (por si lo necesitás en otros flujos).
        /// </summary>
        public bool CambiarDisponibilidadInmueble(int inmuebleId, int nuevaDisponibilidad, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                using (var cn = new SqlConnection(Conexion.cadena))
                using (var cmd = new SqlCommand("UPDATE dbo.inmueble SET disponibilidad = @d WHERE inmueble_id = @i;", cn))
                {
                    cmd.Parameters.AddWithValue("@d", nuevaDisponibilidad);
                    cmd.Parameters.AddWithValue("@i", inmuebleId);
                    cn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }


        public bool FinalizarSiTodasPagadas(int contratoId, out string mensaje)
        {
            mensaje = string.Empty;

            const string sqlCheck = @"
        SELECT 
            SUM(CASE WHEN estado = 2 THEN 1 ELSE 0 END) AS Pagadas,   -- 2 = Pagada
            COUNT(*) AS Total
        FROM dbo.cuota
        WHERE contrato_id = @id;
        ";

            using (var cn = new SqlConnection(Conexion.cadena))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        int pagadas = 0, total = 0;

                        using (var cmd = new SqlCommand(sqlCheck, cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", contratoId);
                            using (var dr = cmd.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    pagadas = dr["Pagadas"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Pagadas"]);
                                    total = dr["Total"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Total"]);
                                }
                            }
                        }

                        if (total == 0)
                        {
                            mensaje = "El contrato no posee cuotas.";
                            tx.Rollback();
                            return false;
                        }

                        if (pagadas != total)
                        {
                            mensaje = "Aún existen cuotas sin pagar. No se puede finalizar.";
                            tx.Rollback();
                            return false;
                        }

                        // Todas pagadas: marcar contrato como Finalizado (2)
                        using (var cmdUp = new SqlCommand(@"
        UPDATE dbo.contrato_alquiler
        SET estado = 2
        WHERE contrato_id = @id AND estado = 1;  -- estaba Vigente
        ", cn, tx))
                        {
                            cmdUp.Parameters.AddWithValue("@id", contratoId);
                            cmdUp.ExecuteNonQuery();
                        }

                        // Devolver disponibilidad del inmueble
                        using (var cmdGet = new SqlCommand("SELECT inmueble_id FROM dbo.contrato_alquiler WHERE contrato_id = @id;", cn, tx))
                        {
                            cmdGet.Parameters.AddWithValue("@id", contratoId);
                            var val = cmdGet.ExecuteScalar();
                            if (val != null)
                            {
                                int inmuebleId = Convert.ToInt32(val);
                                using (var cmdInm = new SqlCommand("UPDATE dbo.inmueble SET disponibilidad = 1 WHERE inmueble_id = @i;", cn, tx))
                                {
                                    cmdInm.Parameters.AddWithValue("@i", inmuebleId);
                                    cmdInm.ExecuteNonQuery();
                                }
                            }
                        }

                        tx.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        mensaje = "Error al finalizar contrato: " + ex.Message;
                        return false;
                    }
                }
            }
        }
    }
}
