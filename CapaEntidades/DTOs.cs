using System;

public class ContratoGridRow
{
    public int Id { get; set; }
    public string Inquilino { get; set; }
    public string Direccion { get; set; }
    public string Inmueble { get; set; }
    public decimal PrecioCuota { get; set; }
    public int CantCuotas { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    // Valor que viene de la tabla/consulta (mensual). Puede venir como 10 o 0.10
    public decimal MoraMensual { get; set; }

    // Por si ya lo estabas usando en otra parte; puede quedar o quitarse si no lo usás
    public decimal MoraDiaria { get; set; }

    // === Calculado: Monto de mora diaria en $ ===
    public decimal MoraDiariaMonto
    {
        get
        {
            if (PrecioCuota <= 0) return 0m;

            // normalizar: si viene 10 => 0.10; si viene 0.10 queda igual
            var pctMensual = MoraMensual > 1m ? MoraMensual / 100m : MoraMensual;

            var pctDiario = pctMensual / 30m;
            return PrecioCuota * pctDiario;
        }
    }

    public int Estado { get; set; }
}
