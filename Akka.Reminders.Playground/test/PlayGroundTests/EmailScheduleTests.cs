using System;
using Akka.Actor;
using Akka.Persistence.Reminders;
using Akka.TestKit;
using Akka.TestKit.Xunit;
using PlaygroundDomain;
using Xunit;
using Xunit.Abstractions;

namespace PlayGroundTests
{
    public class EmailJobTests : TestKit
    {
        
        public EmailJobTests(ITestOutputHelper testOutputHelper)
            : base(Config.Configa, testOutputHelper)
        {
            
        }
        
        [Fact]
        public void when_email_is_scheduled_is_sent_when_is_due()
        {
            var settings = ReminderSettings.Default
                .WithPersistenceId("email")
                .WithTickInterval(TimeSpan.FromMilliseconds(500))
                .WithSnapshotInterval(5);
            var taskId = Guid.NewGuid().ToString();
            var probe = CreateTestProbe("email-sender");
            var scheduler = (TestScheduler)Sys.Scheduler;
            var emailScheduler = Sys.ActorOf(Props.Create(() => new EmailScheduler(settings)).WithDispatcher(CallingThreadDispatcher.Id), "email");
            var when = DateTime.UtcNow.AddDays(1);
            var job = new ScheduleEmail("me", "you", "hi");
            var ack = new Ack(taskId);
            var schedule = new Reminder.Schedule(taskId, probe.Ref.Path, job, when, ack);
            
            emailScheduler.Tell(schedule, probe);
            probe.ExpectMsg<Ack>(x => x.Id == taskId);
            scheduler.AdvanceTo(when);

            probe.ExpectMsg<ScheduleEmail>(x=> x.Body == job.Body && x.From == job.From && x.To == job.To);
        }
        
        [Fact]
        public void when_email_is_scheduled_is_sent_when_is_due_repeatedly()
        {
            var settings = ReminderSettings.Default
                .WithPersistenceId("email")
                .WithTickInterval(TimeSpan.FromMilliseconds(500))
                .WithSnapshotInterval(5);
            var taskId = Guid.NewGuid().ToString();
            var probe = CreateTestProbe("email-sender");
            var scheduler = (TestScheduler)Sys.Scheduler;
            var emailScheduler = Sys.ActorOf(Props.Create(() => new EmailScheduler(settings)).WithDispatcher(CallingThreadDispatcher.Id), "email");
            var when = DateTime.UtcNow.AddDays(1);
            var job = new ScheduleEmail("me", "you", "hi");
            var ack = new Ack(taskId);
            var schedule = new Reminder.ScheduleRepeatedly(taskId, probe.Ref.Path, job, when,TimeSpan.FromHours(1), ack);
            
            emailScheduler.Tell(schedule, probe);
            probe.ExpectMsg<Ack>(x => x.Id == taskId);
            scheduler.AdvanceTo(when);

            probe.ExpectMsg<ScheduleEmail>(x=> x.Body == job.Body && x.From == job.From && x.To == job.To);
            
            scheduler.Advance(TimeSpan.FromHours(1));
            
            probe.ExpectMsg<ScheduleEmail>(x=> x.Body == job.Body && x.From == job.From && x.To == job.To);
            
            scheduler.Advance(TimeSpan.FromHours(1));
            
            probe.ExpectMsg<ScheduleEmail>(x=> x.Body == job.Body && x.From == job.From && x.To == job.To);
            
            scheduler.Advance(TimeSpan.FromHours(1));
            
            probe.ExpectMsg<ScheduleEmail>(x=> x.Body == job.Body && x.From == job.From && x.To == job.To);
        }
    }
}