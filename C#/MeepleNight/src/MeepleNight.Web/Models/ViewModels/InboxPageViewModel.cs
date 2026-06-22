using MeepleNight.Domain.Enums;
using MeepleNight.Services.Dtos;

namespace MeepleNight.Web.Models.ViewModels;

public class InboxPageViewModel
{
    public List<InboxInvitationDto> Pending { get; set; } = new();
    public List<InboxInvitationDto> Upcoming { get; set; } = new();
    public List<InboxInvitationDto> History { get; set; } = new();

    public static InboxPageViewModel From(IEnumerable<InboxInvitationDto> all)
    {
        DateTime now = DateTime.UtcNow;
        InboxPageViewModel page = new();

        foreach (InboxInvitationDto i in all.OrderBy(x => x.ScheduledForUtc))
        {
            if (i.Status == InvitationStatus.Pending && i.ScheduledForUtc > now)
            {
                page.Pending.Add(i);
            }
            else if (i.Status == InvitationStatus.Accepted && i.ScheduledForUtc > now)
            {
                page.Upcoming.Add(i);
            }
            else
            {
                page.History.Add(i);
            }
        }

        return page;
    }
}
