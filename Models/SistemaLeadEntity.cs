namespace MiProyectoJ8.Models
{
    public class SistemaLeadEntity
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public int Edad { get; set; }

        public string Email { get; set; }

        public string Dirección { get; set; }

        public int Telefono { get; set; }

        public DateTime Fechadecreacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public bool EstadoUsuario { get; set; }

        public string pais { get; set; }

        public string Intereses { get; set; }

        public string Rol { get; set; }

        public string FuenteWeb { get; set; }

        
    }
}
