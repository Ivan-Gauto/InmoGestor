using System;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Backup
    {
        // Ya no se usa: private CD_Conexion conexion = new CD_Conexion();
        private string dbName = "inmogestor_db";

        public void ObtenerEstadisticasBackups(out int total, out int exitosos, out int errores, out decimal espacioUsado)
        {
            total = 0;
            exitosos = 0;
            errores = 0;
            espacioUsado = 0;

            // Cambio aquí: Se usa Conexion.cadena
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                string query = @"
                    SELECT 
                        COUNT(backup_id) AS Total,
                        ISNULL(SUM(CASE WHEN estado = 'Completado' THEN 1 ELSE 0 END), 0) AS Exitosos,
                        ISNULL(SUM(CASE WHEN estado = 'Error' THEN 1 ELSE 0 END), 0) AS Errores,
                        ISNULL(SUM(tamaño_mb), 0) AS EspacioUsadoMB
                    FROM 
                        dbo.historial_backup";

                SqlCommand cmd = new SqlCommand(query, oconexion);
                cmd.CommandType = CommandType.Text;

                try
                {
                    oconexion.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total = Convert.ToInt32(reader["Total"]);
                            exitosos = Convert.ToInt32(reader["Exitosos"]);
                            errores = Convert.ToInt32(reader["Errores"]);
                            espacioUsado = Convert.ToDecimal(reader["EspacioUsadoMB"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public DataTable ListarBackups(DateTime fechaFiltro)
        {
            DataTable dt = new DataTable();
            // Cambio aquí: Se usa Conexion.cadena
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                string query = @"
                    SELECT 
                        nombre_archivo, 
                        fecha_hora, 
                        tamaño_mb, 
                        duracion_segundos, 
                        estado, 
                        ruta_archivo, 
                        mensaje_error 
                    FROM 
                        dbo.historial_backup 
                    WHERE 
                        CONVERT(date, fecha_hora) = @fechaFiltro 
                    ORDER BY 
                        fecha_hora DESC";

                SqlCommand cmd = new SqlCommand(query, oconexion);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@fechaFiltro", fechaFiltro.Date);

                try
                {
                    oconexion.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return dt;
            }
        }

        public bool EjecutarBackupSQL(string rutaArchivo, out string mensajeError)
        {
            mensajeError = string.Empty;
            
            // Cambio aquí: Se usa Conexion.cadena
            var builder = new SqlConnectionStringBuilder(Conexion.cadena);
            builder.InitialCatalog = "master";
            string masterConnectionString = builder.ConnectionString;

            string sqlBackupQuery = $"BACKUP DATABASE [{dbName}] TO DISK = @backupPath WITH FORMAT, NAME = N'Backup de {dbName}';";

            try
            {
                using (var connection = new SqlConnection(masterConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sqlBackupQuery, connection))
                    {
                        command.CommandTimeout = 300;
                        command.Parameters.AddWithValue("@backupPath", rutaArchivo);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;
                return false;
            }
        }

        public void RegistrarEnHistorial(string nombreArchivo, DateTime fechaHora, decimal tamañoMb, int duracionSeg, string estado, string rutaArchivo, string mensajeError)
        {
            try
            {
                // Cambio aquí: Se usa Conexion.cadena
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    string sqlInsertQuery = @"
                        INSERT INTO dbo.historial_backup 
                            (nombre_archivo, fecha_hora, tamaño_mb, duracion_segundos, estado, ruta_archivo, mensaje_error)
                        VALUES 
                            (@nombre_archivo, @fecha_hora, @tamaño_mb, @duracion_segundos, @estado, @ruta_archivo, @mensaje_error)";

                    SqlCommand cmd = new SqlCommand(sqlInsertQuery, oconexion);
                    cmd.CommandType = CommandType.Text;
                    
                    cmd.Parameters.AddWithValue("@nombre_archivo", nombreArchivo);
                    cmd.Parameters.AddWithValue("@fecha_hora", fechaHora);
                    cmd.Parameters.AddWithValue("@tamaño_mb", tamañoMb);
                    cmd.Parameters.AddWithValue("@duracion_segundos", duracionSeg);
                    cmd.Parameters.AddWithValue("@estado", estado);
                    cmd.Parameters.AddWithValue("@ruta_archivo", rutaArchivo);
                    
                    if (string.IsNullOrEmpty(mensajeError))
                    {
                        cmd.Parameters.AddWithValue("@mensaje_error", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@mensaje_error", mensajeError);
                    }

                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al registrar en log: " + ex.Message);
            }
        }
    }
}