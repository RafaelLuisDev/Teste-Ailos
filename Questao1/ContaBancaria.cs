using System;
using System.Globalization;

namespace Questao1
{
    class ContaBancaria
    {
        public int Conta { get; set; }
        public string Nome { get; set; }
        public double Valor { get; set; }
        public ContaBancaria(int conta, string nome, double valor)
        {
            Conta = conta;
            Nome = nome;
            Valor = valor;
        }

        public ContaBancaria(int conta, string nome)
        {
            Conta = conta;
            Nome = nome;
        }

        public void Saque(double quantia)
        {
            Valor -= quantia + 3.50;
        }

        public void Deposito(double quantia)
        {
            Valor += quantia;
        }

        public override string ToString()
        {
            return $"Conta {Conta}, Titular: {Nome}, Saldo: $ {Valor:N2}";
        }
    }
}
