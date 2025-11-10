using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_Reportes
    {
        public DataTable ObtenerLogErrorAgrupado(DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        SELECT 
                            COUNT(*) AS Cantidad,
                            MAX(le.fecha_hora) AS UltimaVez,
                            le.formulario AS Formulario,
                            le.metodo AS Metodo,
                            le.mensaje_error AS MensajeError
                        FROM dbo.log_error AS le
                        WHERE CAST(le.fecha_hora AS DATE) BETWEEN @fechaInicio AND @fechaFin
                        GROUP BY le.formulario, le.metodo, le.mensaje_error
                        ORDER BY Cantidad DESC, UltimaVez DESC;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio.Date);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin.Date);
                    oconexion.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error en CD_Reportes (LogErrorAgrupado): " + ex.Message);
                }
                return dt;
            }
        }

        public DataTable ObtenerEstadoDeCuenta(int contratoId, DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                SELECT 
                    c.periodo AS Periodo,
                    p.fecha_pago AS Fecha,
                    'Cuota ' + CAST(c.nro_cuota AS VARCHAR) + ' - ' + c.periodo AS DetalleConcepto,
                    c.importe_base + c.otros_adicionales_total AS Debe,
                    p.monto_total AS Haber,
                    CASE 
                        WHEN c.estado = 0 THEN 'Pendiente'
                        WHEN c.estado = 1 THEN 'Pagada (Pend. Aprob.)'
                        WHEN c.estado = 2 THEN 'Pagada (Confirmada)'
                        WHEN c.estado = 3 THEN 'Anulada'
                        ELSE 'Otro'
                    END AS EstadoDelPago
                FROM 
                    dbo.cuota AS c
                LEFT JOIN 
                    dbo.pago AS p ON c.cuota_id = p.cuota_id AND p.estado IN (1, 2)
                WHERE 
                    c.contrato_id = @contratoId
                    -- --- LÍNEA AÑADIDA ---
                    AND c.fecha_venc BETWEEN @fechaInicio AND @fechaFin
                ORDER BY 
                    c.periodo ASC;
            ";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@contratoId", contratoId);
                    // --- LÍNEAS AÑADIDAS ---
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio.Date);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin.Date);

                    oconexion.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error en CD_Reportes (EstadoDeCuenta): " + ex.Message);
                }
                return dt;
            }
        }

        public List<LogUsuario> ListarAuditoriaUsuarios(string adminDni, DateTime inicio, DateTime fin)
        {
            var lista = new List<LogUsuario>();
            using (var cn = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    var query = new StringBuilder(@"
                        SELECT log_usuario_id, fecha_hora, admin_ejecutor_dni, 
                               usuario_afectado_dni, accion, detalle
                        FROM dbo.log_usuario
                        WHERE fecha_hora BETWEEN @inicio AND @fin
                    ");

                    if (!string.IsNullOrEmpty(adminDni))
                    {
                        query.Append(" AND admin_ejecutor_dni = @adminDni");
                    }
                    query.Append(" ORDER BY fecha_hora DESC");

                    SqlCommand cmd = new SqlCommand(query.ToString(), cn);
                    cmd.Parameters.AddWithValue("@inicio", inicio);
                    cmd.Parameters.AddWithValue("@fin", fin);
                    if (!string.IsNullOrEmpty(adminDni))
                    {
                        cmd.Parameters.AddWithValue("@adminDni", adminDni);
                    }

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new LogUsuario
                            {
                                LogUsuarioId = Convert.ToInt64(dr["log_usuario_id"]),
                                FechaHora = Convert.ToDateTime(dr["fecha_hora"]),
                                AdminEjecutorDni = dr["admin_ejecutor_dni"].ToString(),
                                UsuarioAfectadoDni = dr["usuario_afectado_dni"].ToString(),
                                Accion = dr["accion"].ToString(),
                                Detalle = dr["detalle"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error en CD_Reportes (ListarAuditoriaUsuarios): " + ex.Message);
                }
            }
            return lista;
        }

        public DataTable ObtenerRendimientoOperadores(DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                string query = @"
                    ;WITH Pagos AS (
                        SELECT
                            p.usuario_creador, SUM(p.monto_total) AS IngresosTotales
                        FROM dbo.pago p
                        WHERE p.estado = 1 AND p.fecha_pago BETWEEN @FechaInicio AND @FechaFin
                        GROUP BY p.usuario_creador
                    ),
                    Contratos AS (
                        SELECT
                            c.usuario_creador, COUNT(c.contrato_id) AS ContratosNuevos
                        FROM dbo.contrato_alquiler c
                        WHERE c.fecha_creacion BETWEEN @FechaInicio AND @FechaFin
                        GROUP BY c.usuario_creador
                    )
                    SELECT
                        (pe.nombre + ' ' + pe.apellido) AS Operador,
                        ISNULL(c.ContratosNuevos, 0) AS ContratosNuevos,
                        0 AS ContratosRenovados,
                        ISNULL(p.IngresosTotales, 0) AS IngresosTotalesGestionados,
                        0 AS MontoDeudaTotal
                    FROM dbo.usuario u
                    JOIN dbo.persona pe ON u.dni = pe.dni
                    LEFT JOIN Pagos p ON u.usuario_id = p.usuario_creador
                    LEFT JOIN Contratos c ON u.usuario_id = c.usuario_creador
                    WHERE u.rol_usuario_id = 3
                    ORDER BY Operador;";
                try
                {
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Date);
                    oconexion.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error en CD_Reportes (Rendimiento): " + ex.Message);
                }
                return dt;
            }
        }

        public DataTable ObtenerReporteOcupacion()
        {
            DataTable dt = new DataTable();
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                string query = @"
                DECLARE @WindowEnd DATE = GETDATE();
                DECLARE @WindowStart DATE = DATEADD(year, -1, @WindowEnd);
                DECLARE @TotalDaysInWindow DECIMAL(10, 2) = DATEDIFF(day, @WindowStart, @WindowEnd);
                WITH ActiveProperties AS (
                    SELECT i.inmueble_id, i.direccion, ti.nombre AS TipoInmueble
                    FROM dbo.inmueble i
                    LEFT JOIN dbo.tipo_inmueble ti ON i.id_tipo_inmueble = ti.tipo_inmueble_id
                    WHERE i.estado = 1
                ), ContractPeriods AS (
                    SELECT inmueble_id,
                           CASE WHEN fecha_inicio < @WindowStart THEN @WindowStart ELSE fecha_inicio END AS EffectiveStart,
                           CASE WHEN fecha_fin > @WindowEnd THEN @WindowEnd ELSE fecha_fin END AS EffectiveEnd
                    FROM dbo.contrato_alquiler
                    WHERE estado = 1 AND fecha_inicio <= @WindowEnd AND fecha_fin >= @WindowStart
                ), Occupancy AS (
                    SELECT inmueble_id, SUM(DATEDIFF(day, EffectiveStart, EffectiveEnd) + 1) AS TotalOccupiedDays
                    FROM ContractPeriods
                    GROUP BY inmueble_id
                )
                SELECT ap.direccion AS Inmueble, ap.TipoInmueble AS Tipo,
                       ISNULL(o.TotalOccupiedDays, 0) AS DiasOcupados,
                       (@TotalDaysInWindow - ISNULL(o.TotalOccupiedDays, 0)) AS DiasVacantes,
                       CAST((ISNULL(o.TotalOccupiedDays, 0) / @TotalDaysInWindow) * 100 AS DECIMAL(5, 2)) AS PorcentajeOcupacion
                FROM ActiveProperties ap
                LEFT JOIN Occupancy o ON ap.inmueble_id = o.inmueble_id
                ORDER BY PorcentajeOcupacion ASC;
                ";
                try
                {
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error en CD_Reportes (Ocupación): " + ex.Message);
                }
                return dt;
            }
        }

        public void RegistrarError(int? usuarioId, string formulario, string metodo, string mensaje, string stackTrace)
        {
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        INSERT INTO dbo.log_error (usuario_id, formulario, metodo, mensaje_error, stack_trace)
                        VALUES (@usuarioId, @formulario, @metodo, @mensajeError, @stackTrace);
                    ";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@usuarioId", (object)usuarioId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@formulario", formulario);
                    cmd.Parameters.AddWithValue("@metodo", metodo);
                    cmd.Parameters.AddWithValue("@mensajeError", mensaje);
                    cmd.Parameters.AddWithValue("@stackTrace", (object)stackTrace ?? DBNull.Value);

                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error CRÍTICO al registrar en log_error: " + ex.Message);
                }
            }
        }

        public void RegistrarAuditoriaUsuario(string adminDni, string afectadoDni, string accion, string detalle)
        {
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        INSERT INTO dbo.log_usuario (admin_ejecutor_dni, usuario_afectado_dni, accion, detalle)
                        VALUES (@adminDni, @afectadoDni, @accion, @detalle);
                    ";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@adminDni", (object)adminDni ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@afectadoDni", (object)afectadoDni ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@accion", accion);
                    cmd.Parameters.AddWithValue("@detalle", (object)detalle ?? DBNull.Value);

                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error CRÍTICO al registrar en log_usuario: " + ex.Message);
                }
            }
        }
    }
}