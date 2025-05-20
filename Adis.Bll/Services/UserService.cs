using Adis.Bll.Dtos.User;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Services
{
    /// <inheritdoc cref="IUserService"/>
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(
            IMapper mapper,
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Возникает когда данные пользователя не прошли валидацию</exception>
        public async Task<UserDto> AddUserAsync(UserDto userDto)
        {
            await ValidateUserAsync(userDto);

            var existingUser = await _userRepository.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
                throw new ArgumentException("Пользователь с таким email уже существует");

            var user = _mapper.Map<User>(userDto);
            user.UserName = userDto.Email;

            await _userRepository.CreateUserAsync(user, userDto.Password);
            await _userRepository.AddToRoleAsync(user, userDto.Role.ToString());

            return (await GetUserByIdAsync(user.Id))!;
        }

        private async Task ValidateUserAsync(UserDto userDto)
        {
            if (!new EmailAddressAttribute().IsValid(userDto.Email))
                throw new ArgumentException("Некорректный email");

            var roleName = userDto.Role.ToString();
            if (!await _roleRepository.ExistsAsync(roleName))
                throw new ArgumentException("Роль не существует");

            if (userDto.FullName == null && userDto.Role != Role.Admin)
                throw new ArgumentException("ФИО обязательно для неадминистраторов");
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            return _mapper.Map<IEnumerable<UserDto>>(await _userRepository.GetUsersWithRoleAsync());
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            return _mapper.Map<UserDto>(await _userRepository.GetUserWithRoleByIdAsync(id));
        }

        public async Task<IEnumerable<UserDto>> GetUsersByPartialFullNameWithRoleAsync(string partialFullName, string role)
        {
            if (!Role.TryParse(typeof(Role), role, out var verifedRole))
                new ArgumentException("Такой роли нету");
            return _mapper.Map<IEnumerable<UserDto>>(await _userRepository.GetUsersByPartialFullNameWithRoleAsync(partialFullName, (Role)verifedRole!));
        }

        public async Task<UserDto> UpdateUserAsync(PutUserDto userDto)
        {
            await ValidateUserAsync(_mapper.Map<UserDto>(userDto));

            var existingUser = await _userRepository.GetByIdAsync(userDto.Id);
            if (existingUser.Email != userDto.Email)
            {
                var existingByEmailUser = await _userRepository.FindByEmailAsync(userDto.Email);
                if (existingByEmailUser != null)
                    throw new ArgumentException("Пользователь с таким email уже существует");
            }

            _mapper.Map(userDto, existingUser);
            await _userRepository.UpdateUserAsync(existingUser, userDto.Password);

            var currentRole = (await GetUserByIdAsync(userDto.Id))!.Role.ToString();
            await _userRepository.RemoveFromRoleAsync(existingUser, currentRole!);
            await _userRepository.AddToRoleAsync(existingUser, userDto.Role.ToString()!);

            return (await GetUserByIdAsync(existingUser.Id))!;
        }

        public async Task DeleteUserAsync(int id)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                throw new KeyNotFoundException($"Пользователь с id {id} не найден");

            var currentRole = (await GetUserByIdAsync(id))!.Role.ToString();
            await _userRepository.RemoveFromRoleAsync(existingUser, currentRole!);
            await _userRepository.DeleteUserAsync(existingUser);
        }
    }
}
