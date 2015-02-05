using System.Net.Http;
using System.Web.Routing;
using MvcRouteTester;
using site.Controllers;
using Xunit;

namespace site.tests.App_Start {
    public class RouteTests {
        private readonly RouteCollection _rotas;

        public RouteTests() {
            _rotas = new RouteCollection();
            RouteConfig.RegisterRoutes(_rotas);
        }

        [Fact]
        public void Testa_rota_base() {
            _rotas.ShouldMap("/")
                .To<HomeController>(HttpMethod.Get, h => h.Index(null));
        }

        [Fact]
        public void Testa_rota_pesquisa() {
            _rotas.ShouldMap("/Home/Index/123456789")
                .To<HomeController>(HttpMethod.Get, h => h.Index("123456789"));
        }
        
        [Fact]
        public void Testa_rota_novo_funcionario() {
            _rotas.ShouldMap("/Home/novofuncionario").To<HomeController>(HttpMethod.Get, h => h.NovoFuncionario());
        }
        
        [Fact]
        public void Testa_rota_edicao_funcionario() {
            _rotas.ShouldMap("/Home/fichafuncionario?id=1").To<HomeController>(HttpMethod.Get, h => h.FichaFuncionario(1));
        } 
        
        [Fact]
        public void Testa_rota_modificacao_dados_gerais() {
            _rotas.ShouldMap("/Home/dadosgerais?id=1&versao=1&nome=Luis&nif=123456789&tipoFuncionario=1")
                .To<HomeController>(HttpMethod.Post, h => h.DadosGerais(1,1, "Luis", "123456789", 1));
        } 
        
        [Fact]
        public void Testa_rota_adiciona_contacto() {
            _rotas.ShouldMap("/Home/adicionacontacto?id=1&versao=1&contacto=123456789")
                .To<HomeController>(HttpMethod.Post, h => h.AdicionaContacto(1,1, "123456789"));
        }
        
        [Fact]
        public void Testa_rota_elimina_contacto() {
            _rotas.ShouldMap("/Home/eliminacontacto?id=1&versao=1&contacto=123456789")
                .To<HomeController>(HttpMethod.Post, h => h.EliminaContacto(1,1, "123456789"));
        }
    }
}