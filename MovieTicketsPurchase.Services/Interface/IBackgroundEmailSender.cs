﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Services.Interface
{
    public interface IBackgroundEmailSender
    {
        Task DoWork();
    }
}
