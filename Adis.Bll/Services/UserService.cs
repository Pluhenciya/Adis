using Adis.Bll.Dtos;
using Adis.Bll.Helpers;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Services
{
    /// <inheritdoc cref="IUserService"/>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper) 
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Возникает когда данные пользователя не прошли валидацию</exception>
        public async Task<UserDto> AddUserAsync(UserDto userDto)
        {
            if (await _userRepository.GetUserByEmailAsync(userDto.Email) != null)
                throw new ArgumentException("Эта почта уже занята другим пользователем");

            userDto.PasswordHash = HashHelper.ComputeSha256Hash(userDto.PasswordHash);

            return _mapper.Map<UserDto>(await _userRepository.AddAsync(_mapper.Map<User>(userDto)));
        }
    }
}
