using System;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Reportes
    {
        // --- MÉTODO 1: Rendimiento de Operadores ---
        // MODIFICADO: La consulta ahora devuelve columnas que coinciden con tu DataGridView
        // (Nota: 'ContratosRenovados' y 'Deuda' no se pueden calcular directamente
        // desde un operador, así que los he omitido por ahora).
        public DataTable ObtenerRendimientoOperadores(DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                // Esta consulta une pagos (Ingresos) y contratos (Nuevos)
                string query = @"
                    ;WITH Pagos AS (
                        SELECT
                            p.usuario_creador,
                            SUM(p.monto_total) AS IngresosTotales
                        FROM dbo.pago p
                        WHERE p.estado = 1 AND p.fecha_pago BETWEEN @FechaInicio AND @FechaFin
                        GROUP BY p.usuario_creador
                    ),
                    Contratos AS (
                        SELECT
                            c.usuario_creador,
                            COUNT(c.contrato_id) AS ContratosNuevos
                        FROM dbo.contrato_alquiler c
                        WHERE c.fecha_creacion BETWEEN @FechaInicio AND @FechaFin
                        GROUP BY c.usuario_creador
                    )
                    SELECT
                        (pe.nombre + ' ' + pe.apellido) AS Operador,
                        ISNULL(c.ContratosNuevos, 0) AS ContratosNuevos,
                        0 AS ContratosRenovados, -- Campo no disponible en la BD
                        ISNULL(p.IngresosTotales, 0) AS IngresosTotalesGestionados,
                        0 AS MontoDeudaTotal -- Este KPI no aplica por operador
                    FROM dbo.usuario u
                    JOIN dbo.persona pe ON u.dni = pe.dni
                    LEFT JOIN Pagos p ON u.usuario_id = p.usuario_creador
                    LEFT JOIN Contratos c ON u.usuario_id = c.usuario_creador
                    WHERE u.rol_usuario_id = 3 -- Asumiendo 3 = Rol Operador
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
    }
}