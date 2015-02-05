using FluentNHibernate.Testing;
using site.Models;
using Xunit;

namespace site.tests.Models.NHibernate {
    public class MapeamentoFuncionario {
        [Fact]
        public void Testa_mapemento_funcionar() {
            new AuxiliarTransacao().ExecutaTransacao((sess, tran) => {
                var tipo = new TipoFuncionario {Id = 100, Descricao = "teste"};
                sess.Save(tipo);
                var func = new Funcionario {
                    Contactos = new[] {new Contacto {TipoContacto = TipoContacto.Email, Valor  ="teste@gmail.com"}},
                    Nif = "123456789",
                    Nome = "Luis",
                    TipoFuncionario = tipo
                };
                new PersistenceSpecification<Funcionario>(sess)
                    .VerifyTheMappings(func);
            });
        }
    }
}