using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using AutoMapper;
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
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserService(
            UserManager<User> userManager,
            RoleManager<AppRole> roleManager,
            IMapper mapper,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Возникает когда данные пользователя не прошли валидацию</exception>
        public async Task<UserDto> AddUserAsync(UserDto userDto)
        {
            // Валидация входных данных
            if (!new EmailAddressAttribute().IsValid(userDto.Email))
                throw new ArgumentException("Некорректный email");

            if (string.IsNullOrWhiteSpace(userDto.Role.ToString()))
                throw new ArgumentException("Роль обязательна для заполнения");

            // Проверка существования пользователя
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
                throw new ArgumentException("Пользователь с таким email уже существует");

            if (userDto.FullName == null && userDto.Role != Role.Admin)
                throw new ArgumentException("Пользователь без ФИО, если он не администратор нельзя");

            // Создание пользователя
            var user = _mapper.Map<User>(userDto);
            user.UserName = userDto.Email; // Для Identity требуется UserName

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
                throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Назначение роли
            var roleResult = await _userManager.AddToRoleAsync(user, userDto.Role.ToString());
            if (!roleResult.Succeeded)
                throw new ArgumentException("Ошибка назначения роли");

            var createdUser = _mapper.Map<UserDto>(user);
            createdUser.Role = userDto.Role;
            return createdUser;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            return _mapper.Map<IEnumerable<UserDto>>(await _userRepository.GetUsersWithRoleAsync());
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            return _mapper.Map<UserDto>(await _userRepository.GetUserWithRoleByIdAsync(id));
        }
    }
}
