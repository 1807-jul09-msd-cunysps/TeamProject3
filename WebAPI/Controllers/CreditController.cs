using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebAPI.Models;

namespace ReturnCreditHistory.Controllers
{
    [EnableCors("*","*","*")]
    public class CreditController : ApiController
    {
        CreditHistoryDB db = new CreditHistoryDB();

        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            CreditHistory credit = db.CreditHistories.Where(c => c.SSN == id).FirstOrDefault();

            if (credit == null)
            {
                Random number = new Random();
                credit = new CreditHistory
                {
                    SSN = id,
                    RiskScore = number.Next(1, 101)
                };
                db.CreditHistories.Add(credit);
                db.SaveChanges();
            }
            return Ok(credit.RiskScore);

        }
    }
}
