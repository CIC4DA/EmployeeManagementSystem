﻿using BaseLibrary.Entities.OtherEntities.Sanction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanctionTypeController(IGenericRepositoryInterface<SanctionType> genericRepositoryInterface)
        : GenericController<SanctionType>(genericRepositoryInterface)
    {
    }
}
