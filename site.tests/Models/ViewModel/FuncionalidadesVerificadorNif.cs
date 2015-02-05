using site.Models;
using site.Models.ViewModel;
using site.tests.Models.NHibernate;
using Xunit;

namespace site.tests.Models.ViewModel {
    public class FuncionalidadesVerificadorNif {
        private readonly AuxiliarTransacao _auxiliar = new AuxiliarTransacao();

        [Fact]
        public void Cenario_NIF_duplicado() {
            var nif = "123456789";
            using (var session = _auxiliar.ObtemFabricaSessoes().OpenSession()) {
                using (var tran = session.BeginTransaction()) {
                    var func = Funcionario.CriaVazio(new TipoFuncionario {Id = 1});
                    func.Nif = nif;
                    session.SaveOrUpdate(func);
                    session.Flush();

                    var idFuncionario = func.IdFuncionario + 1;
                    var duplicado = new VerificadorNif(session).NifDuplicado(nif, idFuncionario);
                    Assert.True(duplicado);
                }
            }
        }
        
        [Fact]
        public void Cenario_NIF_nao_duplicado() {
            var nif = "123456789";
            using (var session = _auxiliar.ObtemFabricaSessoes().OpenSession()) {
                using (var tran = session.BeginTransaction()) {
                    var func = Funcionario.CriaVazio(new TipoFuncionario {Id = 1});
                    func.Nif = nif;
                    session.SaveOrUpdate(func);
                    session.Flush();

                    var duplicado = new VerificadorNif(session).NifDuplicado(nif, func.IdFuncionario);
                    Assert.False(duplicado);
                }
            }
        }
    }
}