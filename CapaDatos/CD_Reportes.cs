using System;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Reportes
    {


        // --- MÉTODO 2: Ocupación y Vacancia ---
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
                    SELECT 
                        i.inmueble_id, i.direccion, ti.nombre AS TipoInmueble
                    FROM dbo.inmueble i
                    LEFT JOIN dbo.tipo_inmueble ti ON i.id_tipo_inmueble = ti.tipo_inmueble_id
                    WHERE i.estado = 1
                ),
                ContractPeriods AS (
                    SELECT 
                        inmueble_id,
                        CASE WHEN fecha_inicio < @WindowStart THEN @WindowStart ELSE fecha_inicio END AS EffectiveStart,
                        CASE WHEN fecha_fin > @WindowEnd THEN @WindowEnd ELSE fecha_fin END AS EffectiveEnd
                    FROM dbo.contrato_alquiler
                    WHERE estado = 1 AND fecha_inicio <= @WindowEnd AND fecha_fin >= @WindowStart
                ),
                Occupancy AS (
                    SELECT
                        inmueble_id,
                        SUM(DATEDIFF(day, EffectiveStart, EffectiveEnd) + 1) AS TotalOccupiedDays
                    FROM ContractPeriods
                    GROUP BY inmueble_id
                )
                SELECT 
                    ap.direccion AS Inmueble,
                    ap.TipoInmueble AS Tipo,
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



        /// <summary>
        /// Reporte: Ocupación y Vacancia por inmueble en un rango (por defecto últimos 90 días).
        /// Calcula días ocupados sumando los solapes de contratos (estado IN (1,2)) con el rango.
        /// </summary>
        public DataTable ObtenerRendimientoOperadores(DateTime desde, DateTime hasta)
        {
            var dt = new DataTable();

            string sql = @"
;WITH contratos_rango AS (
    SELECT 
        c.contrato_id,
        c.usuario_creador,
        c.inmueble_id,
        c.dni_inquilino,
        c.fecha_inicio,
        c.fecha_fin,
        c.estado
    FROM dbo.contrato_alquiler c
    WHERE c.fecha_inicio >= @desde
      AND c.fecha_inicio <= @hasta
),
contratos_marcados AS (
    SELECT 
        cr.*,
        CASE WHEN EXISTS (
            SELECT 1
            FROM dbo.contrato_alquiler prev
            WHERE prev.dni_inquilino = cr.dni_inquilino
              AND prev.inmueble_id  = cr.inmueble_id
              AND prev.estado       = 2
              AND prev.fecha_fin    <= cr.fecha_inicio
        ) THEN 1 ELSE 0 END AS EsRenovado
    FROM contratos_rango cr
),
agg_contratos AS (
    SELECT 
        cm.usuario_creador,
        COUNT(*) AS TotalContratos,
        SUM(CASE WHEN cm.EsRenovado = 1 THEN 1 ELSE 0 END) AS Renovados
    FROM contratos_marcados cm
    GROUP BY cm.usuario_creador
),
agg_ingresos AS (
    SELECT 
        cr.usuario_creador,
        SUM(pg.monto_total) AS IngresosTotalesGestionados
    FROM contratos_rango cr
    JOIN dbo.cuota q
      ON q.contrato_id = cr.contrato_id
    JOIN dbo.pago pg
      ON pg.cuota_id = q.cuota_id
     AND pg.estado = 1
    -- Si querés que los ingresos respeten el rango por fecha de pago, descomentá:
    -- AND pg.fecha_pago >= @desde
    -- AND pg.fecha_pago <= @hasta
    GROUP BY cr.usuario_creador
),
agg_deuda AS (
    SELECT
        cr.usuario_creador,
        SUM(q2.importe_base) AS MontoDeudaTotal
    FROM contratos_rango cr
    JOIN dbo.cuota q2
      ON q2.contrato_id = cr.contrato_id
    WHERE q2.estado = 0
      AND q2.fecha_venc < CAST(GETDATE() AS date)
    GROUP BY cr.usuario_creador
),
operadores AS (
    SELECT 
        u.usuario_id,
        (p.apellido + ', ' + p.nombre) AS Operador
    FROM dbo.usuario u
    INNER JOIN dbo.persona p ON p.dni = u.dni
    WHERE u.rol_usuario_id IN (2,3) -- Gerente/Operador (ajustá si querés incluir otros)
)
SELECT
    op.Operador,
    ISNULL(ac.TotalContratos - ac.Renovados, 0) AS ContratosNuevos,
    ISNULL(ac.Renovados, 0)                    AS ContratosRenovados,
    ISNULL(ai.IngresosTotalesGestionados, 0)   AS IngresosTotalesGestionados,
    ISNULL(ad.MontoDeudaTotal, 0)              AS MontoDeudaTotal
FROM operadores op
LEFT JOIN agg_contratos ac ON ac.usuario_creador = op.usuario_id
LEFT JOIN agg_ingresos  ai ON ai.usuario_creador = op.usuario_id
LEFT JOIN agg_deuda     ad ON ad.usuario_creador = op.usuario_id
ORDER BY op.Operador;";

            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(sql, cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@desde", SqlDbType.Date).Value = desde.Date;
                cmd.Parameters.Add("@hasta", SqlDbType.Date).Value = hasta.Date;

                // --- Si aún se rompe, dejá este try/catch para ver el error exacto en un MessageBox ---
                try
                {
                    da.Fill(dt);
                }
                catch (SqlException ex)
                {
                    // MOSTRÁ el error real (así salimos de la ceguera de que solo “rompe en Fill”)
                    System.Windows.Forms.MessageBox.Show(
                        "SQL Error: " + ex.Message,
                        "Reporte Rendimiento - SQL",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error
                    );
                    throw; // re-lanzá si querés que burbujee
                }
            }
            return dt;
        }

        // using System.Data;
        // using System.Data.SqlClient;

        public DataTable ObtenerInquilinosMorosos(DateTime desde, DateTime hasta, string dniInquilino, int? contratoId)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            using (SqlCommand cmd = new SqlCommand(@"
;WITH cuotas_vencidas AS
(
    SELECT
        ca.contrato_id,
        ca.dni_inquilino,
        (p.nombre + ' ' + p.apellido)       AS Inquilino,
        p.telefono                           AS Telefono,
        i.direccion                          AS Inmueble,
        ISNULL(i.division,'')                AS NroDepartamento,
        COUNT(*)                             AS PeriodosAdeudados,
        SUM(c.importe_base + ISNULL(c.otros_adicionales_total,0)) AS MontoTotalAdeudado,
        MAX(DATEDIFF(DAY, c.fecha_venc, GETDATE()))              AS DiasAtraso
    FROM cuota c
    INNER JOIN contrato_alquiler ca ON ca.contrato_id = c.contrato_id
    INNER JOIN persona p            ON p.dni = ca.dni_inquilino
    INNER JOIN inmueble i           ON i.inmueble_id = ca.inmueble_id
    -- Regla de morosidad: cuota vencida y sin pagar
    WHERE c.estado = 0                      -- pendiente/vencida
      AND c.fecha_venc < GETDATE()         -- ya venció
      AND c.fecha_venc BETWEEN @desde AND @hasta
      AND (@dni IS NULL OR ca.dni_inquilino = @dni)
      AND (@contratoId IS NULL OR ca.contrato_id = @contratoId)
    GROUP BY ca.contrato_id, ca.dni_inquilino, p.nombre, p.apellido, p.telefono, i.direccion, i.division
)
SELECT
    Inquilino,
    Telefono,
    Inmueble,
    NroDepartamento      AS [Nro. departamento],
    PeriodosAdeudados    AS [Periodos adeudados],
    MontoTotalAdeudado   AS [Monto total adeudado],
    DiasAtraso           AS [Dias de atraso]
FROM cuotas_vencidas
ORDER BY Inquilino;", cn))
            {
                cmd.Parameters.Add("@desde", SqlDbType.Date).Value = desde.Date;
                cmd.Parameters.Add("@hasta", SqlDbType.Date).Value = hasta.Date;
                cmd.Parameters.Add("@dni", SqlDbType.VarChar, 50).Value = (object)dniInquilino ?? DBNull.Value;
                cmd.Parameters.Add("@contratoId", SqlDbType.Int).Value = (object)contratoId ?? DBNull.Value;

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                return dt;
            }
        }





    }
}