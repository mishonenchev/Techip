﻿using MOFO.Database.Contracts;
using MOFO.Models;
using MOFO.Services.Addons;
using MOFO.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MOFO.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public void AddUser(User user)
        {
            _userRepository.Add(user);
            _userRepository.SaveChanges();
        }
        public void RemoveUser(User user)
        {
            _userRepository.Remove(user);
            _userRepository.SaveChanges();
        }
        public User GetUserByAuth(string auth)
        {
            return _userRepository.WhereIncludeAll(x => x.Auth == auth).FirstOrDefault();
        }
        public List<User> GetAll()
        {
            return _userRepository.GetAll().ToList();
        }
        public string NewAuthString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = "";
            Random rn = new Random();
            for (int i = 0; i < 64; i++)
            {
                result += chars[rn.Next(0, chars.Length - 1)];
            }
            if (_userRepository.Where(x => x.Auth == result).Count() == 0)
            {
                return result;
            }
            else return NewAuthString();
        }
        public bool IsTelephoneValid(string telephone)
        {
            if (telephone[0] == '0' && (telephone[1] == '8' || telephone[1] == '9'))
            {
                return telephone.Length == 10;
            }
            else if (telephone[0] == '0' && telephone[1] == '0')
            {
                telephone = telephone.Remove(0, 2);
            }
            Regex digitsOnly = new Regex(@"[^\d]");
            var onlyDigitPhone = digitsOnly.Replace(telephone, "");

            var code = "";
            for (int i = 0; i < onlyDigitPhone.Length - 9; i++)
            {
                code += onlyDigitPhone[i];
            }
            var barephone = "";
            for (int i = code.Length; i < onlyDigitPhone.Length; i++)
            {
                barephone += onlyDigitPhone[i];
            }
            if (TelephoneCountryCodes.IsCodeValid(code))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public void Update()
        {
            _userRepository.SaveChanges();
        }
    }
}
