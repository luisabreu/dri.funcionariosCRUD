using System;
using FluentAssertions;
using Moq;
using NHibernate;
using site.Models;
using site.Models.ViewModel;
using Xbehave;

namespace site.tests.Models.ViewModel {
    public class FuncionalidadesRepositorioFuncionarios {
        private readonly AutoMockContainer _container = new AutoMockContainer(new MockRepository(MockBehavior.Strict));

        [Scenario]
        public void Cenario_com_sucesso(RepositorioFuncionarios repositorio, Funcionario funcionario) {
            "Dado um repositorio"
                .Given(() => repositorio = _container.Create<RepositorioFuncionarios>());

            "E um funcionário com NIF não repetido"
                .And(() => funcionario = Funcionario.CriaVazio(new TipoFuncionario()));

            "E alguns mocks"
                .And(() => {
                    _container.GetMock<IVerificadorNif>()
                        .Setup(v => v.NifDuplicado(It.IsAny<string>(), It.IsAny<int>()))
                        .Returns(false);
                    _container.GetMock<ISession>()
                        .Setup(s => s.SaveOrUpdate(It.IsAny<object>()));
                });

            "Quando gravamos o funcionário"
                .When(() => repositorio.Grava(funcionario));

            "Então~não obtemos exceção e serviço verificação é usado"
                .Then(() => {
                    _container.GetMock<IVerificadorNif>().VerifyAll();
                    _container.GetMock<ISession>().VerifyAll();
                });
        }
        
        [Scenario]
        public void Cenario_sem_sucesso(RepositorioFuncionarios repositorio, 
            Funcionario funcionario,
            InvalidOperationException excecaoEsperada) {
            "Dado um repositorio"
                .Given(() => repositorio = _container.Create<RepositorioFuncionarios>());

            "E um funcionário com NIF repetido"
                .And(() => funcionario = Funcionario.CriaVazio(new TipoFuncionario()));

            "E alguns mocks"
                .And(() => _container.GetMock<IVerificadorNif>()
                    .Setup(v => v.NifDuplicado(It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(true));

            "Quando gravamos o funcionário"
                .When(() => {
                    try {
                        repositorio.Grava(funcionario);
                    }
                    catch (InvalidOperationException ex) {
                        excecaoEsperada = ex;
                    }
                });

            "Então obtemos uma exceção "
                .Then(() => excecaoEsperada.Should().NotBeNull());

            "e serviço verificação é usado"
                .And(() => _container.GetMock<IVerificadorNif>().VerifyAll());
        }
    }
}