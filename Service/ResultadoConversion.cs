namespace Service
{
    /// <summary>
    /// Qué hay que hacer en pantalla para convertir la sílaba: borrar los últimos
    /// <paramref name="CaracteresABorrar"/> caracteres y escribir <paramref name="TextoAEscribir"/>.
    /// Solo se borra desde el primer carácter que cambia (en "hao3" se borra "ao" y se escribe "ǎo").
    /// </summary>
    public record ResultadoConversion(int CaracteresABorrar, string TextoAEscribir);
}
