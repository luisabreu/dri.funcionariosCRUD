using FluentNHibernate.Testing;
using site.Models;
using Xunit;

namespace site.tests.Models.NHibernate {
    public class MapeamentoTipoFuncionario {
        [Fact]
        public void Deve_mapear_tipo_funcionario() {
            new AuxiliarTransacao().ExecutaTransacao((sess, tran) => {
                new PersistenceSpecification<TipoFuncionario>(sess)
                    .VerifyTheMappings(new TipoFuncionario {Id = 100, Descricao = "Luis"});
            });
        }
    }
}