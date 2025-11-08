// Program.cs
// Module09: Facade + Composite example in one C# file
// Требуется: .NET SDK (6.0 или новее). Запуск: dotnet run

using System;
using System.Collections.Generic;
using System.Linq;

namespace Module09
{
    // ---------------------------
    // Facade: подсистемы мультимедиа
    // ---------------------------
    class TV
    {
        public bool IsOn { get; private set; } = false;
        public string Channel { get; private set; } = "1";

        public void TurnOn()
        {
            if (!IsOn) { IsOn = true; Console.WriteLine("TV: Включён"); }
            else Console.WriteLine("TV: Уже включён");
        }

        public void TurnOff()
        {
            if (IsOn) { IsOn = false; Console.WriteLine("TV: Выключен"); }
            else Console.WriteLine("TV: Уже выключен");
        }

        public void SetChannel(string channel)
        {
            Channel = channel;
            Console.WriteLine($"TV: Установлен канал {channel}");
        }
    }

    class AudioSystem
    {
        public bool IsOn { get; private set; } = false;
        private int volume = 20;
        public int Volume { get => volume; }

        public void TurnOn()
        {
            if (!IsOn) { IsOn = true; Console.WriteLine("AudioSystem: Включена"); }
            else Console.WriteLine("AudioSystem: Уже включена");
        }

        public void TurnOff()
        {
            if (IsOn) { IsOn = false; Console.WriteLine("AudioSystem: Выключена"); }
            else Console.WriteLine("AudioSystem: Уже выключена");
        }

        public void SetVolume(int vol)
        {
            vol = Math.Max(0, Math.Min(100, vol));
            volume = vol;
            Console.WriteLine($"AudioSystem: Громкость установлена на {volume}");
        }
    }

    class DVDPlayer
    {
        private bool playing = false;

        public void Play()
        {
            if (!playing) { playing = true; Console.WriteLine("DVDPlayer: Воспроизведение"); }
            else Console.WriteLine("DVDPlayer: Уже воспроизводится");
        }

        public void Pause()
        {
            if (playing) { playing = false; Console.WriteLine("DVDPlayer: Пауза"); }
            else Console.WriteLine("DVDPlayer: Нечего ставить на паузу");
        }

        public void Stop()
        {
            if (playing) playing = false;
            Console.WriteLine("DVDPlayer: Стоп");
        }
    }

    class GameConsole
    {
        public bool IsOn { get; private set; } = false;

        public void TurnOn()
        {
            if (!IsOn) { IsOn = true; Console.WriteLine("GameConsole: Включена"); }
            else Console.WriteLine("GameConsole: Уже включена");
        }

        public void TurnOff()
        {
            if (IsOn) { IsOn = false; Console.WriteLine("GameConsole: Выключена"); }
            else Console.WriteLine("GameConsole: Уже выключена");
        }

        public void StartGame(string gameName)
        {
            if (!IsOn) TurnOn();
            Console.WriteLine($"GameConsole: Запуск игры '{gameName}'");
        }
    }

    class HomeTheaterFacade
    {
        private TV tv;
        private AudioSystem audio;
        private DVDPlayer dvd;
        private GameConsole console;

        public HomeTheaterFacade(TV tv, AudioSystem audio, DVDPlayer dvd, GameConsole console)
        {
            this.tv = tv;
            this.audio = audio;
            this.dvd = dvd;
            this.console = console;
        }

        public void WatchMovie(string channel = "1", int volume = 40)
        {
            Console.WriteLine("\n--- Подготовка к просмотру фильма ---");
            tv.TurnOn();
            tv.SetChannel(channel);
            audio.TurnOn();
            audio.SetVolume(volume);
            dvd.Play();
            Console.WriteLine("--- Приятного просмотра ---\n");
        }

        public void EndMovie()
        {
            Console.WriteLine("\n--- Завершаем просмотр ---");
            dvd.Stop();
            audio.TurnOff();
            tv.TurnOff();
            Console.WriteLine("--- Система выключена ---\n");
        }

        public void PlayGame(string gameName, int volume = 30)
        {
            Console.WriteLine("\n--- Подготовка к игре ---");
            tv.TurnOn();
            audio.TurnOn();
            console.TurnOn();
            console.StartGame(gameName);
            audio.SetVolume(volume);
            Console.WriteLine("--- Приятной игры ---\n");
        }

        public void ListenMusic(int volume = 25)
        {
            Console.WriteLine("\n--- Включаем режим прослушивания музыки ---");
            tv.TurnOn(); // имитация источника
            audio.TurnOn();
            audio.SetVolume(volume);
            Console.WriteLine("--- Музыка воспроизводится ---\n");
        }

        public void SetVolume(int vol) => audio.SetVolume(vol);
    }

    // ---------------------------
    // Composite: файловая система
    // ---------------------------
    abstract class FileSystemComponent
    {
        public string Name { get; protected set; }
        public FileSystemComponent(string name) { Name = name; }

        public abstract long GetSize();
        public abstract void Display(string indent = "");
    }

    class FileComponent : FileSystemComponent
    {
        private long size;
        public FileComponent(string name, long size) : base(name) { this.size = size; }

        public override long GetSize() => size;

        public override void Display(string indent = "")
        {
            Console.WriteLine($"{indent}- File: {Name} ({size} bytes)");
        }
    }

    class DirectoryComponent : FileSystemComponent
    {
        private List<FileSystemComponent> children = new List<FileSystemComponent>();

        public DirectoryComponent(string name) : base(name) { }

        public void Add(FileSystemComponent component)
        {
            // проверка по ссылке или по имени
            if (children.Any(c => ReferenceEquals(c, component) || c.Name == component.Name))
            {
                Console.WriteLine($"Directory '{Name}': Компонент '{component.Name}' уже существует — добавление отменено.");
                return;
            }
            children.Add(component);
            Console.WriteLine($"Directory '{Name}': Добавлен компонент '{component.Name}'");
        }

        public void Remove(FileSystemComponent component)
        {
            if (!children.Remove(component))
            {
                Console.WriteLine($"Directory '{Name}': Компонент '{component.Name}' не найден — удаление отменено.");
                return;
            }
            Console.WriteLine($"Directory '{Name}': Удалён компонент '{component.Name}'");
        }

        public override long GetSize()
        {
            long total = 0;
            foreach (var c in children) total += c.GetSize();
            return total;
        }

        public override void Display(string indent = "")
        {
            Console.WriteLine($"{indent}+ Directory: {Name} ({GetSize()} bytes)");
            foreach (var c in children) c.Display(indent + "   ");
        }
    }

    // ---------------------------
    // Main: демонстрация
    // ---------------------------
    class Program
    {
        static void DemoFacade()
        {
            Console.WriteLine("=== Демонстрация Facade ===");
            var tv = new TV();
            var audio = new AudioSystem();
            var dvd = new DVDPlayer();
            var console = new GameConsole();

            var facade = new HomeTheaterFacade(tv, audio, dvd, console);

            facade.WatchMovie(channel: "5", volume: 45);
            facade.EndMovie();

            facade.PlayGame("Super Fun Game", volume: 20);
            facade.SetVolume(10);

            facade.ListenMusic(volume: 18);
        }

        static void DemoComposite()
        {
            Console.WriteLine("\n=== Демонстрация Composite ===");
            var root = new DirectoryComponent("root");
            var f1 = new FileComponent("readme.txt", 1200);
            var f2 = new FileComponent("photo.jpg", 2500000);

            var photos = new DirectoryComponent("photos");
            var p1 = new FileComponent("vacation1.jpg", 1400000);
            var p2 = new FileComponent("vacation2.jpg", 1600000);

            root.Add(f1);
            root.Add(f2);
            root.Add(photos);

            photos.Add(p1);
            photos.Add(p2);

            // попытка добавить дубликата
            photos.Add(p1);

            root.Display();
            Console.WriteLine($"Общий размер root: {root.GetSize()} bytes");

            // удаляем и повторяем
            photos.Remove(p2);
            root.Display();
            Console.WriteLine($"Общий размер root после удаления: {root.GetSize()} bytes");
        }

        static void Main(string[] args)
        {
            DemoFacade();
            DemoComposite();
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
