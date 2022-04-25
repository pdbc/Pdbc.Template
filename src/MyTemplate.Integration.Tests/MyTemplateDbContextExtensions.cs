using MyTemplate.Data;

namespace MyTemplate.Integration.Tests
{
    public static class MyTemplateDbContextExtensions
    {
        public static void DeleteTimeRegistrationWithId(this MyTemplateDbContext context, long id)
        {
            //var registration = context.Registrations.FirstOrDefault(c => c.Id == id);
            //if (registration != null)
            //{
            //    context.Remove(registration);
            //}
            //context.SaveChanges();


            //var registration = context.TimeRegistrations.FirstOrDefault(a => a.Id == id);
            //if (registration != null) { context.Remove(registration); }
            //context.SaveChanges();
        }
    }


}
