using System;
using krisna_dto.Data;

namespace krisna_dto.Controllers
{
	public class UserController
	{
        private readonly UserData _userData;

        public UserController(UserData userData)
        {
            _userData = userData;
        }


    }
}

