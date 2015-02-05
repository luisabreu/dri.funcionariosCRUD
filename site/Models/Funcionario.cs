using System.Collections.Generic;

namespace site.Models {
    public class Funcionario {

        public int IdFuncionario { get; set; }
        public string Nome { get; set; }
        public string Nif { get; set; }
        public int Versao { get; set; }
        public IList<Contacto> Contactos { get; set; }
        public TipoFuncionario TipoFuncionario { get; set; }

        public static Funcionario CriaVazio(TipoFuncionario tipo) {
            return new Funcionario {
                    Contactos = new List<Contacto>(),
                    IdFuncionario = 0,
                    Nif = "",
                    Nome = "",
                    TipoFuncionario = tipo,
                    Versao = 0
                };
        }
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