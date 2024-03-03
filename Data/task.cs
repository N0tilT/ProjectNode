﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace node1
{

    /// <summary>
    /// Контейнер данных дат для сериализации
    /// </summary>
    public class DateTimeContainerData : DataContainer
    {
        public DateTime Created;
        public DateTime? Updated;
        public DateTime? Planned;
    }

    /// <summary>
    /// Контейнер с датами
    /// </summary>
    public class DateTimeContainer
    {
        /// <summary>
        /// Создание контейнера по-умолчанию с текущей датой
        /// </summary>
        public static DateTimeContainer Now => new DateTimeContainer(DateTime.Now);

        /// <summary>
        /// Создать объект из контейнера с данными
        /// </summary>
        /// <param name="data">Контейнер с данными</param>
        /// <returns>Объект</returns>
        public static DateTimeContainer FromData(DateTimeContainerData data)
        {
            return new DateTimeContainer(data.Created, data.Updated, data.Planned);
        }

        /// <summary>
        /// Дата создания
        /// </summary>
        private DateTime _created;

        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        private DateTime? _updated = null;

        /// <summary>
        /// Запланированная дата
        /// </summary>
        private DateTime? _planned = null;

        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="created">Дата создания</param>
        /// <param name="updated">Дата последнего обновления или null</param>
        /// <param name="planned">Запланированная дата или null</param>
        public DateTimeContainer(DateTime created, DateTime? updated = null, DateTime? planned = null)
        {
            Created = created;
            Updated = updated;
            Planned = planned;
        }

        /// <summary>
        /// Получить контейнер с данными из объекта
        /// </summary>
        /// <returns>Контейнер с данными</returns>
        public DateTimeContainerData ToData()
        {
            return new DateTimeContainerData
            {
                Created = Created,
                Updated = Updated,
                Planned = Planned
            };
        }

        /// <summary>
        /// Доступ к дате создания
        /// </summary>
        public DateTime Created
        {
            get => _created;
            private set
            {
                _created = value;

                // Перепроверяем зависимые даты
                // для предотвращения повреждения данных
                Updated = Updated;
                Planned = Planned;
            }
        }

        /// <summary>
        /// Доступ к дате последнего обновления
        /// </summary>
        public DateTime? Updated
        {
            get => _updated;
            set
            {
                if (value != null && value < Created)
                {
                    throw new ArgumentOutOfRangeException("updated не может быть раньше, чем created");
                }

                _updated = value;
            }
        }

        /// <summary>
        /// Доступ к запланированной дате
        /// </summary>
        public DateTime? Planned
        {
            get => _planned;
            set
            {
                if (value != null && value < Created)
                {
                    throw new ArgumentOutOfRangeException("planned не может быть раньше, чем created");
                }

                _planned = value;
            }
        }

        /// <summary>
        /// Проверить, задана ли дата последнего обновления
        /// </summary>
        /// <returns>Статус проверки</returns>
        public bool HasUpdated()
        {
            return Updated != null;
        }

        /// <summary>
        /// Проверить, задана ли запланированная дата
        /// </summary>
        /// <returns>Статус проверки</returns>
        public bool HasPlanned()
        {
            return Planned != null;
        }
    }

    /// <summary>
    /// Контейнер данных карты для сериализации
    /// </summary>
    public class TaskData : UniqueData
    {
        public DateTimeContainerData Date;
        public string Description;
    }

    /// <summary>
    /// Карта
    /// </summary>
    public class Task : Unique<Task>, IUniqueIdentifiable
    {
        /// <summary>
        /// Создать объект из контейнера с данными
        /// </summary>
        /// <param name="data">Контейнер с данными</param>
        /// <returns>Объект</returns>
        public static Task FromData(TaskData data)
        {
            DateTimeContainer date = DateTimeContainer.FromData(data.Date);

            return new Task(data.Id, data.Name, date, data.Description);
        }

        /// <summary>
        /// Контейнер с датами
        /// </summary>
        private DateTimeContainer _date;

        /// <summary>
        /// Описание
        /// </summary>
        private string _description = String.Empty;

        /// <summary>
        /// Статус важности
        /// </summary>
        private bool _isImportant = false;

        /// <summary>
        /// Статус выполнения
        /// </summary>
        private bool _isCompleted = false;

        /// <summary>
        /// Расширенный конструктор
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Название</param>
        /// <param name="date">Контейнер с датами</param>
        /// <param name="description">Описание</param>
        /// <param name="isImportant">Статус важности</param>
        /// <param name="isCompleted">Статус выполнения</param>
        public Task(int id, string name, DateTimeContainer date, string description) : this(id, name, date)
        {
            Description = description;

        }

        /// <summary>
        /// Базовый конструктор
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Название</param>
        /// <param name="date">Контейнер с датами</param>
        public Task(int id, string name, DateTimeContainer date) : base(id, name)
        {
            Date = date;
        }

        /// <summary>
        /// Базовый конструктор с автоматическим определением идентификатора
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="date">Контейнер с датами или null</param>
        public Task(string name, DateTimeContainer date = null) : base(name)
        {
            Date = date ?? DateTimeContainer.Now;
        }

        /// <summary>
        /// Получить контейнер с данными из объекта
        /// </summary>
        /// <returns>Контейнер с данными</returns>
        public TaskData ToData()
        {
            return new TaskData
            {
                Id = Id,
                Name = Name,
                Date = Date.ToData(),
                Description = Description,
            };
        }

        /// <summary>
        /// Доступ к контейнеру с датами
        /// </summary>
        public DateTimeContainer Date
        {
            get => _date;
            private set => _date = value;
        }

        /// <summary>
        /// Доступ к описанию
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = value.Trim();
        }

        public override string ToString()
        {
            return ToData().ToString();
        }
    }

}
