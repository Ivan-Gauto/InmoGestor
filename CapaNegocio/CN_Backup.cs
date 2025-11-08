using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using CapaDatos;

namespace CapaNegocio
{
    public class CN_Backup
    {
        private CD_Backup objCD_Backup = new CD_Backup();

        public void CargarTarjetas(out int total, out int exitosos, out int errores, out decimal espacioUsado)
        {
            objCD_Backup.ObtenerEstadisticasBackups(out total, out exitosos, out errores, out espacioUsado);
        }

        public DataTable ListarBackups(DateTime fechaFiltro)
        {
            return objCD_Backup.ListarBackups(fechaFiltro);
        }

        public bool RealizarBackup(string rutaArchivo, out string mensajeError)
        {
            string nombreArchivo = Path.GetFileName(rutaArchivo);
            DateTime horaInicio = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch();
            long tamañoBytes = 0;
            int duracionSeg = 0;
            bool exitoSQL = false;
            string errorSQL = string.Empty;

            try
            {
                stopwatch.Start();
                exitoSQL = objCD_Backup.EjecutarBackupSQL(rutaArchivo, out errorSQL);
                stopwatch.Stop();

                duracionSeg = (int)stopwatch.Elapsed.TotalSeconds;

                if (exitoSQL)
                {
                    FileInfo fileInfo = new FileInfo(rutaArchivo);
                    tamañoBytes = fileInfo.Length;
                    decimal tamañoMb = (decimal)tamañoBytes / (1024 * 1024);

                    objCD_Backup.RegistrarEnHistorial(nombreArchivo, horaInicio, tamañoMb, duracionSeg, "Completado", rutaArchivo, null);
                    mensajeError = string.Empty;
                    return true;
                }
                else
                {
                    objCD_Backup.RegistrarEnHistorial(nombreArchivo, horaInicio, 0, duracionSeg, "Error", rutaArchivo, errorSQL);
                    mensajeError = errorSQL;
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }
                duracionSeg = (int)stopwatch.Elapsed.TotalSeconds;
                mensajeError = ex.Message;
                objCD_Backup.RegistrarEnHistorial(nombreArchivo, horaInicio, 0, duracionSeg, "Error", rutaArchivo, ex.Message);
                return false;
            }
        }
    }
}