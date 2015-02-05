using System.Collections.Generic;

namespace site.Models {
    public class Funcionario {

        public int IdFuncionario { get; set; }
        public string Nome { get; set; }
        public string Nif { get; set; }
        public int Versao { get; set; }
        public IList<Contacto> Contactos { get; set; }
        public TipoFuncionario TipoFuncionario { get; set; }

    }

    public class Contacto {
        public string Valor { get; set; }
        public TipoContacto TipoContacto { get; set; }
    }

    public class TipoFuncionario {
        public int Id { get; set; }
        public string Descricao { get; set; }
    }

    public enum TipoContacto {
        Telefone,
        Email,
        Extensao
    }

}