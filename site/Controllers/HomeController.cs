using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using NHibernate;
using site.Models;
using site.Models.ViewModel;

namespace site.Controllers {
    public class HomeController : Controller {
        private static Regex _nifRegex = new Regex(@"^\d{9}$");
        private readonly RepositorioFuncionarios _repositorioFuncionarios;
        private readonly RepositorioTiposFuncionario _repositorioTiposFuncionario;
        private readonly ISession _session;

        public HomeController(ISession session,
            RepositorioFuncionarios repositorioFuncionarios,
            RepositorioTiposFuncionario repositorioTiposFuncionario) {
            Contract.Requires(session != null);
            Contract.Requires(repositorioFuncionarios != null);
            Contract.Requires(repositorioTiposFuncionario != null);
            Contract.Ensures(_session != null);
            Contract.Ensures(_repositorioFuncionarios != null);
            _session = session;
            _repositorioFuncionarios = repositorioFuncionarios;
            _repositorioTiposFuncionario = repositorioTiposFuncionario;
        }

        public ActionResult Index(string nifOuNome) {
            if (string.IsNullOrEmpty(nifOuNome)) {
                return View(new DadosPesquisa {NifOuNome = ""});
            }
            using (var tran = _session.BeginTransaction()) {
                return View(new DadosPesquisa {
                    Funcionarios = _repositorioFuncionarios.Pesquisa(nifOuNome),
                    NifOuNome = nifOuNome,
                    PesquisaEfetuada = true
                });
            }
        }

        public ActionResult NovoFuncionario() {
            using (var tran = _session.BeginTransaction()) {
                var tipos = _repositorioTiposFuncionario.ObtemTodosTipos();
                var funcionario = Funcionario.CriaVazio(tipos.First());

                return View("fichafuncionario", new DadosFichaFormulario {
                    ENovo = true,
                    Funcionario = funcionario,
                    TiposFuncionario = tipos
                });
            }
        }

        public ActionResult FichaFuncionario(int id) {
            using (var tran = _session.BeginTransaction()) {
                var funcionario = _repositorioFuncionarios.ObtemFuncionario(id);
                var tipos = _repositorioTiposFuncionario.ObtemTodosTipos();

                return View("fichafuncionario", new DadosFichaFormulario {
                    ENovo = false,
                    Funcionario = funcionario,
                    TiposFuncionario = tipos
                });
            }
        }

        public ActionResult DadosGerais(int id, int versao, string nome, string nif, int tipoFuncionario) {
            return id == 0 && versao == 0
                ? AdicionaNovoFuncionario(nome, nif, tipoFuncionario)
                : ModificaDadosGerais(id, versao, nome, nif, tipoFuncionario);
        }

        private ActionResult ModificaDadosGerais(int id, int versao, string nome, string nif, int tipoFuncionario) {
            using (var tran = _session.BeginTransaction()) {
                var tipos = _repositorioTiposFuncionario.ObtemTodosTipos();
                var tipo = tipos.First(tf => tf.Id == tipoFuncionario);

                var funcionario = _repositorioFuncionarios.ObtemFuncionario(id);
                Contract.Assume(funcionario != null);

                funcionario.Nif = nif;
                funcionario.Nome = nome;
                funcionario.TipoFuncionario = tipo;

                try {
                    _repositorioFuncionarios.Grava(funcionario);
                    tran.Commit();
                }
                catch (InvalidOperationException) {
                    ModelState.AddModelError("nif", "NIF já foi associado a outro contribuinte");
                }

               

                return View("fichafuncionario", new DadosFichaFormulario {
                    ENovo = false,
                    Funcionario = funcionario,
                    TiposFuncionario = tipos
                });
            }
        }

        private ActionResult AdicionaNovoFuncionario(string nome, string nif, int tipoFuncionario) {
            using (var tran = _session.BeginTransaction()) {
                var ocorreuErro = false;
                var tipos = _repositorioTiposFuncionario.ObtemTodosTipos();
                var tipo = tipos.First(tf => tf.Id == tipoFuncionario);
                var funcionario = new Funcionario {
                    Nome = nome,
                    Nif = nif,
                    TipoFuncionario = tipo,
                    Contactos = new List<Contacto>()
                };

                try {
                    _repositorioFuncionarios.Grava(funcionario);
                    tran.Commit();
                }
                catch (InvalidOperationException) {
                    ModelState.AddModelError("nif", "NIF já foi associado a outro contribuinte");
                    ocorreuErro = true;
                }
                return View("fichafuncionario", new DadosFichaFormulario {
                    ENovo = ocorreuErro,
                    Funcionario = funcionario,
                    TiposFuncionario = tipos
                });
            }
        }

        

        public ActionResult AdicionaContacto(int id, int versao, string contacto) {
            using (var tran = _session.BeginTransaction()) {
                var tipos = _repositorioTiposFuncionario.ObtemTodosTipos();
                var funcionario = _repositorioFuncionarios.ObtemFuncionario(id);

                Contract.Assume(funcionario.Versao == versao);

                var ct = ObtemContacto(contacto);
                Contract.Assume(ct != null);
                funcionario.Contactos.Add(ct);

                tran.Commit();
                return View("fichafuncionario", new DadosFichaFormulario {
                    ENovo = false,
                    Funcionario = funcionario,
                    TiposFuncionario = tipos
                });
            }
        }

        private Contacto ObtemContacto(string contacto) {
            if (new Regex(@"^\d{9}$").IsMatch(contacto)) {
                return new Contacto {
                    TipoContacto = TipoContacto.Telefone,
                    Valor = contacto
                };
            }
            if (new Regex(@"^\d{4}$").IsMatch(contacto)) {
                return new Contacto {
                    TipoContacto = TipoContacto.Extensao,
                    Valor = contacto
                };
            }
            try {
                new MailAddress(contacto);
                return new Contacto {
                    TipoContacto = TipoContacto.Email,
                    Valor = contacto
                };
            }
            catch (Exception) {
                return null;
            }
        }

        public ActionResult EliminaContacto(int id, int versao, string contacto) {
            using (var tran = _session.BeginTransaction()) {
                var tipos = _repositorioTiposFuncionario.ObtemTodosTipos();
                var funcionario = _session.Load<Funcionario>(id);
                Contract.Assume(funcionario != null && funcionario.Versao == versao);

                for (var i = 0; i < funcionario.Contactos.Count; i++) {
                    var ct = funcionario.Contactos[i];
                    if (ct.Valor == contacto) {
                        funcionario.Contactos.RemoveAt(i);
                        break;
                    }
                }
    
                tran.Commit();
                return View("fichafuncionario", new DadosFichaFormulario {
                    ENovo = false,
                    Funcionario = funcionario,
                    TiposFuncionario = tipos
                });
            }
        }


        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_session != null);
            Contract.Invariant(_repositorioFuncionarios != null);
        }
    }
}