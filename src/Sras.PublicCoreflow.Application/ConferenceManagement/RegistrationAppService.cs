using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class RegistrationAppService : PublicCoreflowAppService, IRegistrationAppService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IRepository<Registration, Guid> _registrationRepository;
        private readonly IRepository<Order, Guid> _orderRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IConferenceRepository _conferenceRepository;
        private readonly IRepository<ConferenceAccount, Guid> _conferenceAccountRepository;

        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;

        public RegistrationAppService(
            ISubmissionRepository submissionRepository,
            IRepository<Registration, Guid> registrationRepository,
            IRepository<Order, Guid> orderRepository,
            IRepository<IdentityUser, Guid> userRepository,
            IConferenceRepository conferenceRepository,
            IRepository<ConferenceAccount, Guid> conferenceAccountRepository,
            ICurrentUser currentUser, 
            IGuidGenerator guidGenerator)
        {
            _submissionRepository = submissionRepository;
            _registrationRepository = registrationRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _conferenceRepository = conferenceRepository;
            _conferenceAccountRepository = conferenceAccountRepository;

            _currentUser = currentUser;
            _guidGenerator = guidGenerator;
        }

        public async Task<RegistrablePaperTable> GetRegistrablePaperTable(Guid conferenceId, Guid accountId)
        {
            return await _submissionRepository.GetRegistrablePaperTable(conferenceId, accountId);
        }

        public async Task<RegistrationResponseDto> CreateRegistration(Guid accountId, Guid conferenceId, string mainPaperOption, List<RegistrationInput> registrations)
        {
            var account = await _userRepository.FindAsync(accountId);
            if(account == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.AccountNotFound);
            }

            var conference = await _conferenceRepository.GetAsync(conferenceId);
            if (conference == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);
            }

            var conferenceAccount =
                await _conferenceAccountRepository.GetAsync(x => x.AccountId == accountId
                && x.ConferenceId == conferenceId);

            // Simulation only
            PriceTable? priceTable = null;
            var isSimulation = true;
            if (isSimulation)
            {
                priceTable = ConferenceConsts.DefaultPriceTable;
            }
            else if (conference.RegistrationSettings != null && !conference.RegistrationSettings.IsNullOrWhiteSpace())
            {
                priceTable = JsonSerializer.Deserialize<PriceTable?>(conference.RegistrationSettings);
            }

            mainPaperOption = mainPaperOption ?? string.Empty;
            mainPaperOption = mainPaperOption.Trim();
            var mainPaperPriceTableRow = new PriceTableRow();
            if (priceTable == null || priceTable.Rows == null
                || !priceTable.Rows.Any(x => x.Option.ToLower().Equals(mainPaperOption.ToLower())))
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.MainPaperOptionNotFound);
            }
            else
            {
                mainPaperPriceTableRow = priceTable.Rows.First(x => x.Option.ToLower().Equals(mainPaperOption.ToLower()));
            }
            mainPaperOption = mainPaperPriceTableRow.Option;

            var isEarlyRegistration = priceTable.EarlyRegistrationDeadline != null
                && DateTime.Now < priceTable.EarlyRegistrationDeadline.Value.Date;
            OrderDto order = new OrderDto();
            order.Details = new List<OrderDetail>();
            order.Total = 0;
            OrderDetail mainPaper = new OrderDetail
            {
                ChargeType = OrderConsts.MainPaperChargeType,
                Option = mainPaperOption,
                Price = (isEarlyRegistration
                        ? mainPaperPriceTableRow.EarlyRegistration
                        : mainPaperPriceTableRow.RegularRegistration) ?? 0,
                Amount = 0,
                Subtotal = 0
            };
            bool hasMainPaper = false;
            OrderDetail extraPage = new OrderDetail
            {
                ChargeType = OrderConsts.ExtraPageChargeType,
                Price = (isEarlyRegistration
                        ? priceTable.Rows.First(x => x.Option.ToLower().Contains(OrderConsts.ExtraPageChargeType.ToLower())).EarlyRegistration
                        : priceTable.Rows.First(x => x.Option.ToLower().Contains(OrderConsts.ExtraPageChargeType.ToLower())).RegularRegistration) ?? 0,
                Amount = 0,
                Subtotal = 0
            };
            bool hasExtraPage = false;
            OrderDetail oneExtraPaper = new OrderDetail
            {
                ChargeType = OrderConsts.OneExtraPaperChargeType,
                Price = (isEarlyRegistration
                        ? priceTable.Rows.First(x => x.Option.ToLower().Contains(OrderConsts.OneExtraPaperChargeType.ToLower())).EarlyRegistration
                        : priceTable.Rows.First(x => x.Option.ToLower().Contains(OrderConsts.OneExtraPaperChargeType.ToLower())).RegularRegistration) ?? 0,
                Amount = 0,
                Subtotal = 0
            };
            bool hasOneExtraPaper = false;
            OrderDetail twoExtraPaper = new OrderDetail
            {
                ChargeType = OrderConsts.TwoExtraPaperChargeType,
                Price = (isEarlyRegistration
                        ? priceTable.Rows.First(x => x.Option.ToLower().Contains(OrderConsts.TwoExtraPaperChargeType.ToLower())).EarlyRegistration
                        : priceTable.Rows.First(x => x.Option.ToLower().Contains(OrderConsts.TwoExtraPaperChargeType.ToLower())).RegularRegistration) ?? 0,
                Amount = 0,
                Subtotal = 0
            };
            bool hasTwoExtraPaper = false;

            if (!registrations.Any())
                return new RegistrationResponseDto
                {
                    IsSuccessful = false,
                    Message = "Registration papers empty!"
                };

            var maxValidNumberOfPages = priceTable.MaxValidNumberOfPages;
            Registration newRegistration = new Registration(_guidGenerator.Create(), conferenceAccount.Id);
            registrations.ForEach(x =>
            {
                mainPaper.Amount++;
                mainPaper.Subtotal = mainPaper.Amount * mainPaper.Price;

                if (!hasMainPaper)
                {
                    hasMainPaper = true;
                }

                if (x.MainPaper.NumberOfExtraPages != null && x.MainPaper.NumberOfExtraPages >= 0)
                {
                    if (x.MainPaper.NumberOfPages - x.MainPaper.NumberOfExtraPages != maxValidNumberOfPages && x.MainPaper.NumberOfPages > maxValidNumberOfPages)
                    {
                        throw new BusinessException(PublicCoreflowDomainErrorCodes.InvalidNumberOfExtraPages +
                            $": PaperId {x.MainPaper.SubmissionId}: NumberOfPages minus NumberOfExtraPages should equal {maxValidNumberOfPages}");
                    }

                    if (x.MainPaper.NumberOfExtraPages > 0)
                    {
                        extraPage.Amount += x.MainPaper.NumberOfExtraPages.Value;
                        extraPage.Subtotal = extraPage.Amount * extraPage.Price;

                        if (!hasExtraPage)
                        {
                            hasExtraPage = true;
                        }
                    }
                }

                RegistrationPaper main = new RegistrationPaper(_guidGenerator.Create(), x.MainPaper.SubmissionId, newRegistration.Id,
                    x.MainPaper.NumberOfPages, x.MainPaper.NumberOfExtraPages ?? 0, null, null);
                newRegistration.RegistrationPapers.Add(main);

                // Extra Paper
                switch (x.ExtraPapers.Count)
                {
                    case 1:
                        oneExtraPaper.Amount++;
                        oneExtraPaper.Subtotal = oneExtraPaper.Amount * oneExtraPaper.Price;

                        if (!hasOneExtraPaper)
                        {
                            hasOneExtraPaper = true;
                        }

                        break;
                    case 2:
                        twoExtraPaper.Amount++;
                        twoExtraPaper.Subtotal = twoExtraPaper.Amount * twoExtraPaper.Price;

                        if (!hasTwoExtraPaper)
                        {
                            hasTwoExtraPaper = true;
                        }

                        break;
                    default: break;
                }

                x.ExtraPapers.ForEach(y =>
                {
                    RegistrationPaper extra = new RegistrationPaper(_guidGenerator.Create(), y.SubmissionId, newRegistration.Id,
                        y.NumberOfPages, y.NumberOfExtraPages ?? 0, main.Id, null);
                    newRegistration.RegistrationPapers.Add(extra);

                    if (y.NumberOfExtraPages != null && y.NumberOfExtraPages >= 0 && y.NumberOfPages > maxValidNumberOfPages)
                    {
                        if (y.NumberOfPages - y.NumberOfExtraPages != maxValidNumberOfPages)
                        {
                            throw new BusinessException(PublicCoreflowDomainErrorCodes.InvalidNumberOfExtraPages +
                                $": PaperId {y.SubmissionId}: NumberOfPages minus NumberOfExtraPages should equal {maxValidNumberOfPages}");
                        }

                        if (y.NumberOfExtraPages > 0)
                        {
                            extraPage.Amount += y.NumberOfExtraPages.Value;
                            extraPage.Subtotal = extraPage.Amount * extraPage.Price;

                            if (!hasExtraPage)
                            {
                                hasExtraPage = true;
                            }
                        }
                    }
                });
            });

            try
            {
                if (hasMainPaper)
                {
                    order.Details.Add(mainPaper);
                    order.Total += mainPaper.Subtotal;
                }
                if (hasExtraPage)
                {
                    order.Details.Add(extraPage);
                    order.Total += extraPage.Subtotal;
                }
                if (hasOneExtraPaper)
                {
                    order.Details.Add(oneExtraPaper);
                    order.Total += oneExtraPaper.Subtotal;
                }
                if (hasTwoExtraPaper)
                {
                    order.Details.Add(twoExtraPaper);
                    order.Total += twoExtraPaper.Subtotal;
                }
                    
                var t = await _registrationRepository.InsertAsync(newRegistration);

                var totalWholeAmount = (int)Math.Truncate(order.Total);
                var totalFractionalAmount = (int)Math.Round(OrderConsts.FractionalFactor * (order.Total - totalWholeAmount), 0);
                var newOrder = new Order(_guidGenerator.Create(), accountId,
                    JsonSerializer.Serialize(order), totalWholeAmount, totalFractionalAmount, OrderConsts.DefaultCurrency);
                await _orderRepository.InsertAsync(newOrder);

                return new RegistrationResponseDto
                {
                    IsSuccessful = true,
                    Message = "Created Successfully",
                    RegistrationId = newRegistration.Id,
                    OrderId = newOrder.Id,
                    Order = order
                };
            }
            catch (Exception)
            {
                return new RegistrationResponseDto
                {
                    IsSuccessful = false,
                    Message = "Exception"
                };
            }
        }
    }
}
