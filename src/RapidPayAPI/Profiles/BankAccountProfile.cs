using AutoMapper;
using RapidPayAPI.Dtos;
using RapidPayAPI.Models;

namespace RapidPayAPI.Profiles
{
    public class BankAccountProfile:Profile
    {
        public BankAccountProfile()
        {
          
            CreateMap<BankAccount, BankAccountReadDto>();
            CreateMap<BankAccount, AccountBalanceDto>();
            CreateMap<BankAccountCreateDto,BankAccount>().ForMember(t=>t.Balance,opt=>opt.MapFrom(src=>src.InitialBalance));
            CreateMap<Bank,BankReadDto>();
        }
    }
}