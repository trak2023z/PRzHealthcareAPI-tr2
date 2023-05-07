using AutoMapper;
using PRzHealthcareAPI.Exceptions;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Models.DTO;

namespace PRzHealthcareAPI.Services
{
    public interface INotificationService
    {
        List<NotificationTypeDto> GetAllTypes();
        void EditNotificationType(NotificationTypeDto dto, int accountId);
    }

    public class NotificationService : INotificationService
    {
        private readonly HealthcareDbContext _dbContext;
        private readonly IMapper _mapper;

        public NotificationService(HealthcareDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Pobranie typów notyfikacji
        /// </summary>
        /// <returns>Lista typów notyfikacji</returns>
        public List<NotificationTypeDto> GetAllTypes()
        {
            var list = _dbContext.NotificationTypes.ToList();
            if (list is null)
            {
                return new List<NotificationTypeDto>();
            }

            List<NotificationTypeDto> listNotificationTypeDto = new List<NotificationTypeDto>();

            foreach (var notif in list)
            {
                listNotificationTypeDto.Add(_mapper.Map<NotificationTypeDto>(notif));
            }

            return listNotificationTypeDto;
        }

        /// <summary>
        /// Edycja wybranego typu notyfikacji
        /// </summary>
        /// <param name="dto">Obiekt typu notyfikacji</param>
        /// <param name="accountId">Id zalogowanego użytkownika</param>
        public void EditNotificationType(NotificationTypeDto dto, int accountId)
        {
            var notificationType = _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Id == dto.Id);
            if (notificationType == null)
            {
                throw new NotFoundException("Powiadomienie nie istnieje.");
            }

            notificationType.Nty_Template = dto.Template;

            _dbContext.NotificationTypes.Update(notificationType);
            _dbContext.SaveChanges();
        }
    }
}
