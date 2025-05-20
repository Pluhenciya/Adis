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
            await ValidateUserAsync(userDto);

            // Проверка существования пользователя
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
                throw new ArgumentException("Пользователь с таким email уже существует");

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

            return (await GetUserByIdAsync(user.Id))!;
        }

        private async Task ValidateUserAsync(UserDto userDto)
        {
            // Валидация входных данных
            if (!new EmailAddressAttribute().IsValid(userDto.Email))
                throw new ArgumentException("Некорректный email");

            if (string.IsNullOrWhiteSpace(userDto.Role.ToString()))
                throw new ArgumentException("Роль обязательна для заполнения");

            if (userDto.FullName == null && userDto.Role != Role.Admin)
                throw new ArgumentException("Нельзя не указать у пользователя ФИО, если он не администратор");
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

            if(existingUser.Email != userDto.Email)
            {
                var existingByEmailUser = await _userManager.FindByEmailAsync(userDto.Email);
                if (existingByEmailUser != null)
                    throw new ArgumentException("Пользователь с таким email уже существует");
            }

            _mapper.Map(userDto, existingUser);

            if(userDto.Password != null)
                existingUser.PasswordHash = _userManager.PasswordHasher.HashPassword(existingUser, userDto.Password);

            await _userManager.RemoveFromRoleAsync(existingUser, (await GetUserByIdAsync(userDto.Id))!.Role.ToString()!);

            await _userManager.AddToRoleAsync(existingUser, userDto.Role.ToString()!);

            await _userRepository.UpdateAsync(existingUser);

            return (await GetUserByIdAsync(existingUser.Id))!;
        }

        public async Task DeleteUserAsync(int id)
        {
            var existingUser = await _userManager.FindByIdAsync(id.ToString());
            if (existingUser == null)
                throw new KeyNotFoundException($"Пользователь с id {id} не найден");

            await _userManager.RemoveFromRoleAsync(existingUser, (await GetUserByIdAsync(id))!.Role.ToString()!);

            await _userManager.DeleteAsync(existingUser);
        }
    }
}
