using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataConnection.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewModel.Chat.Request;
using ViewModel.Chat.Response;

namespace Service.ChatService
{
    public interface IChatService
    {
        public Task<IActionResult> CreateChat(ChatInsertViewModel cvm);
        public Task<IActionResult> GetChats(Guid adviseId, int index);
    }
    public class ChatService : IChatService
    {
        private ExpertConnectionContext _context;
        private IMapper _mapper;

        public ChatService(IMapper mapper, ExpertConnectionContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> CreateChat(ChatInsertViewModel cvm)
        {
            var advise = await _context.Advises.Include(x => x.CategoryMapping).FirstOrDefaultAsync(x => x.Id.Equals(cvm.AdviseId.ToString()) && !x.IsDone);
            bool check = true;
            if (advise != null)
            {
                if (advise.UserId.Equals(cvm.FromAcc.ToString()))
                {
                    if (!advise.CategoryMapping.ExpertId.Equals(cvm.ToAcc.ToString()))
                    {
                        check = false;
                    }
                }
                else if (advise.CategoryMapping.ExpertId.Equals(cvm.ToAcc.ToString()))
                {
                    if (!advise.UserId.Equals(cvm.FromAcc.ToString()))
                    {
                        check = false;
                    }
                }
            }
            if (!check)
            {
                return new StatusCodeResult(400);
            }
            Chat chat = new Chat
            {
                Id = Guid.NewGuid().ToString(),
                AdviseId = cvm.AdviseId,
                FromAcc = cvm.FromAcc,
                ToAcc = cvm.ToAcc,
                CreatedDate = DateTime.Now,
                ImageUrl = cvm.ImageUrl ?? null,
                Contents = cvm.Contents
            };
            await _context.Chats.AddAsync(chat);
            return await _context.SaveChangesAsync() > 0 ? new JsonResult(_mapper.Map<ChatViewModel>(chat)) : new StatusCodeResult(500);
        }

        public async Task<IActionResult> GetChats(Guid adviseId, int index)
        {
            int start = (index - 1) * 10;
            int end = index * 10;
            return new JsonResult(await _context.Chats.Where(x => x.AdviseId.Equals(adviseId.ToString())).OrderByDescending(x => x.CreatedDate).Skip(start).Take(end).ProjectTo<ChatViewModel>(_mapper.ConfigurationProvider).ToListAsync());
        }
    }
}
