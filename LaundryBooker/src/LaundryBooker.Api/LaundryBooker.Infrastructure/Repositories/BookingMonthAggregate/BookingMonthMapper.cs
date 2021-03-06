using System.Collections.Generic;
using LaundryBooker.Domain.Model.BookingMonth;
using LaundryBooker.Domain.Model.BookingMonth.Entities;
using LaundryBooker.Domain.Model.User;

namespace LaundryBooker.Infrastructure.Repositories.BookingMonthAggregate
{
    public static class BookingMonthMapper
    {
        public static BookingMonth From(BookingMonthDataModel dataModel)
        {
            if (dataModel == null)
                return null;

            var aggregateId = BookingMonthId.With(dataModel.Id);
            
            return new BookingMonth(
                aggregateId,
                dataModel.Year,
                dataModel.Month,
                From(dataModel.BookingDays));
        }

        private static Dictionary<int, BookingDay> From(Dictionary<int, BookingDayDataModel> dataModel)
        {
            if (dataModel == null)
                return null;

            var entries = new Dictionary<int, BookingDay>();

            foreach (var key in dataModel.Keys)
            {
                entries.Add(key, From(dataModel[key]));
            }

            return entries;
        }

        private static BookingDay From(BookingDayDataModel dataModel)
        {
            if (dataModel == null)
                return null;

            var entityId = BookingDayId.With(dataModel.Id);
            
            var entity = new BookingDay(entityId,From(dataModel.Bookings));

            return entity;
        }

        private static Dictionary<Slot, UserId> From(Dictionary<SlotEnumModel, string> dataModel)
        {
            if (dataModel == null)
                return null;

            var bookings = new Dictionary<Slot, UserId>();
            
            foreach (var key in dataModel.Keys)
            {
                var slot = (Slot) key;
                var userId = UserId.With(dataModel[key]);
                
                bookings.Add(slot,userId);
            }

            return bookings;
        }

        public static BookingMonthDataModel From(BookingMonth aggregate)
        {
            return new BookingMonthDataModel
            {
                Id = aggregate.Id.Value,
                Month = aggregate.Month,
                Year = aggregate.Year,
                BookingDays = From(aggregate.BookingDays)
            };
        }
        
        private static Dictionary<int, BookingDayDataModel> From(Dictionary<int, BookingDay> entity)
        {
            var entries = new Dictionary<int, BookingDayDataModel>();

            foreach (var key in entity.Keys)
            {
                entries.Add(key, From(entity[key]));
            }

            return entries;
        }
        
        private static BookingDayDataModel From(BookingDay entity)
        {
            var dataModelId = entity.Id.Value;
            
            var dataModel = new BookingDayDataModel
            {
                Id = dataModelId,
                Bookings = From(entity.Bookings)
            };

            return dataModel;
        }
        
        private static Dictionary<SlotEnumModel, string> From(Dictionary<Slot, UserId> entity)
        {
            var bookings = new Dictionary<SlotEnumModel, string>();
            
            foreach (var key in entity.Keys)
            {
                var slot = (SlotEnumModel) key;
                var userId = entity[key].Value;
                
                bookings.Add(slot,userId);
            }

            return bookings;
        }
    }
}