﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence.EventStore;
using Akka.Persistence.Journal;
using Akkatecture.Aggregates;
using Akkatecture.Extensions;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleDomain;
using SimpleDomain.Model.UserAccount;
using SimpleDomain.Model.UserAccount.Commands;
using SimpleDomain.Model.UserAccount.Events;

namespace SimpleApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //var upcaster = new UserAccountAggregateEventUpcaster();

            //var oldevt = new CommittedEvent<UserAccountAggregate,UserAccountId,UserAccountNameChangedEvent>(UserAccountId.New,new UserAccountNameChangedEvent("test"),null); 
            //var 
            //var evt1 = upcaster.FromJournal(oldevt, String.Empty);
            //Console.WriteLine("Ok");
            //var evt = upcaster.Upcast(oldevt);
            //Console.WriteLine(evt);
            
            //Create actor system
            var system = ActorSystem.Create("useraccount-example",Config.Postgres);

            //Create supervising aggregate manager for UserAccount aggregate root actors
            var aggregateManager = system.ActorOf(Props.Create(() => new UserAccountAggregateManager()));

            //Build create user account aggregate command
            //var aggregateId = UserAccountId.New;
            //var createUserAccountCommand = new CreateUserAccountCommand(aggregateId, "foo bar");
            
            //Send command, this is equivalent to command.publish() in other cqrs frameworks
            //aggregateManager.Tell(createUserAccountCommand);
            var aggregateId = UserAccountId.With("useraccount-a40e3a87-6ebf-4900-884d-7c17477ea20b");
            while (true)
            {
                var newName = GetRandomString();
                var changeNameCommand = new UserAccountChangeNameCommand(aggregateId, newName);
                aggregateManager.Tell(changeNameCommand);
                await Task.Delay(1000);
            }
            
            //block end of program
            Console.WriteLine("done");
            Console.ReadLine(); 
        }

        public static string GetRandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            
            var random = new Random();
            var stringChars = new char[random.Next(3,12)];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }
    }
}