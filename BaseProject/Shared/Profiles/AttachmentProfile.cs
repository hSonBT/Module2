using AutoMapper;
using BaseProject.Shared.Dtos;
using BaseProject.Shared.Models;

namespace BaseProject.Shared.Profiles;

public class AttachmentProfile : Profile
{
    public AttachmentProfile()
    {
        CreateMap<Attachment, AttachmentDto>();
    }
}