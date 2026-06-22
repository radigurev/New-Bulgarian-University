using AutoMapper;
using MeepleNight.Domain.Entities;
using MeepleNight.Services.Dtos;

namespace MeepleNight.Services.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Game
        CreateMap<Game, GameSummaryDto>();
        CreateMap<Game, GameDetailsDto>();
        CreateMap<CreateGameRequest, Game>()
            .ForMember(g => g.Id, opt => opt.Ignore())
            .ForMember(g => g.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(g => g.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(g => g.CreatedByUserId, opt => opt.Ignore())
            .ForMember(g => g.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(g => g.UpdatedByUserId, opt => opt.Ignore())
            .ForMember(g => g.Sessions, opt => opt.Ignore())
            .ForMember(g => g.Candidates, opt => opt.Ignore());

        CreateMap<UpdateGameRequest, Game>()
            .ForMember(g => g.IsActive, opt => opt.Ignore())
            .ForMember(g => g.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(g => g.CreatedByUserId, opt => opt.Ignore())
            .ForMember(g => g.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(g => g.UpdatedByUserId, opt => opt.Ignore())
            .ForMember(g => g.Sessions, opt => opt.Ignore())
            .ForMember(g => g.Candidates, opt => opt.Ignore());

        CreateMap<Game, CandidateGameDto>()
            .ForMember(d => d.GameId, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.Position, opt => opt.Ignore());

        // GameNight
        CreateMap<GameNight, GameNightSummaryDto>()
            .ForMember(d => d.HostDisplayName,
                opt => opt.MapFrom(src => src.Host == null ? string.Empty : src.Host.DisplayName))
            .ForMember(d => d.AcceptedAttendeeCount, opt => opt.Ignore());

        CreateMap<GameNight, GameNightDetailsDto>()
            .ForMember(d => d.HostDisplayName,
                opt => opt.MapFrom(src => src.Host == null ? string.Empty : src.Host.DisplayName))
            .ForMember(d => d.ViewerIsHost, opt => opt.Ignore())
            .ForMember(d => d.ViewerIsAccepted, opt => opt.Ignore())
            .ForMember(d => d.CandidateGames, opt => opt.Ignore())
            .ForMember(d => d.Invitees, opt => opt.Ignore())
            .ForMember(d => d.Sessions, opt => opt.Ignore());

        // Invitation
        CreateMap<Invitation, AttendeeDto>()
            .ForMember(d => d.InvitationId, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.InviteeUserId))
            .ForMember(d => d.DisplayName,
                opt => opt.MapFrom(src => src.Invitee == null ? string.Empty : src.Invitee.DisplayName))
            .ForMember(d => d.Email,
                opt => opt.MapFrom(src => src.Invitee == null ? null : src.Invitee.Email));

        CreateMap<Invitation, InboxInvitationDto>()
            .ForMember(d => d.InvitationId, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.GameNightTitle,
                opt => opt.MapFrom(src => src.GameNight == null ? string.Empty : src.GameNight.Title))
            .ForMember(d => d.ScheduledForUtc,
                opt => opt.MapFrom(src => src.GameNight == null ? default : src.GameNight.ScheduledForUtc))
            .ForMember(d => d.Location,
                opt => opt.MapFrom(src => src.GameNight == null ? string.Empty : src.GameNight.Location))
            .ForMember(d => d.HostDisplayName, opt => opt.Ignore());

        // Session
        CreateMap<Session, SessionDto>()
            .ForMember(d => d.GameTitle,
                opt => opt.MapFrom(src => src.Game == null ? string.Empty : src.Game.Title));

        CreateMap<SessionPlayer, SessionPlayerDto>()
            .ForMember(d => d.DisplayName,
                opt => opt.MapFrom(src => src.User == null ? string.Empty : src.User.DisplayName));
    }
}
