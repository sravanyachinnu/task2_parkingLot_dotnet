using System;
using System.Collections.Generic;
namespace ParkingLot
{
    enum VehicleType
    {
        TwoWheel,
        FourWheel,
        HeavyVehicle
    }
    /* enum ShowAllParking
     {
         ShowParkingStatus,
         ShowUnParkingStatus
     }*/
    static class TicketGenerator
    {
        private static int ticketNumber = 1;
        public static string GenerateTicket()
        {
            string prefix = "TICKET";
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string ticket = $"{prefix}_{timestamp}_{ticketNumber}";
            ticketNumber++; 
            return ticket;  
        }                    
    }                         
    class Program
    {
        
        static void Main(string[] args)
        {

                bool setupCompleted = false;
                do
                {
                    try
                    {
                        Console.WriteLine("Do you want to set up? (yes/no)");
                        string option = Console.ReadLine()?.ToLower();
                        if (option == "no")
                        {
                            Console.WriteLine("Restarting the setup...");
                            continue;
                        }
                        else if (option != "yes")
                        {
                            throw new InvalidOperationException("Invalid input. Please enter 'yes' or 'no'.");
                        }
                        setupCompleted = true; 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                } while (!setupCompleted);
   

            List<TwoWheel> twoWheels = SetupParking<TwoWheel>("two wheel", "Enter the number of two wheel: ");
            List<FourWheel> fourWheels = SetupParking<FourWheel>("four wheel", "Enter the number of four wheel: ");
            List<HeavyVehicle> heavyVehicles = SetupParking<HeavyVehicle>("heavy vehicle", "Enter the number of heavy vehicle:");

            char choice;
            do
            {
                Console.WriteLine("Parking_lot_types:");
                Console.WriteLine("1. Two wheel");
                Console.WriteLine("2. Four wheel");
                Console.WriteLine("3. Heavy vehicle");
                Console.WriteLine("4. Show all parking places");
                Console.WriteLine("E.exit ");
                Console.WriteLine("Enter your choice:");
                /* choice = char.Parse(Console.ReadLine());*/
                if (!char.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a valid choice.");
                    continue;
                }
                try
                {
                    switch (choice)
                    {
                        case '1':
                            ProcessVehicleParking(twoWheels, VehicleType.TwoWheel);
                            break;
                        case '2':
                            ProcessVehicleParking(fourWheels, VehicleType.FourWheel);
                            break;
                        case '3':
                            ProcessVehicleParking(heavyVehicles, VehicleType.HeavyVehicle);
                            break;
                        case '4':
                            ShowAllParking(twoWheels, fourWheels, heavyVehicles);
                            break;
                       
                        case 'E':
                            Console.WriteLine("thanks for using parking");
                            break;
                        default:
                            throw new InvalidOperationException("Invalid choice.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            } while (choice != 'E');
        }



        static List<T> SetupParking<T>(string parkingType, string message) where T : Vehicle
        {
            try
            {
                Console.WriteLine(message);
                int num = int.Parse(Console.ReadLine());
                List<T> parking = new List<T>();
                parking.AddRange(Enumerable.Range(0, num).Select(_ => (T)Activator.CreateInstance(typeof(T))));
                return parking;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return null;
            }
        }
        static void ShowAllParking(List<TwoWheel> twoWheels, List<FourWheel> fourWheels, List<HeavyVehicle> heavyVehicles)
        {
            Console.WriteLine("All parking places:");
            ShowParkingStatus(twoWheels, "Two Wheel");
            ShowParkingStatus(fourWheels, "Four Wheel");
            ShowParkingStatus(heavyVehicles, "Heavy Vehicle");
        }
        static void ShowParkingStatus<T>(List<T> parking, string vehicleType) where T : Vehicle
        {
            Console.WriteLine(vehicleType + ":");
            parking.Select((v, i) => $"{i + 1}. {v.GetParkingStatus()}").ToList().ForEach(Console.WriteLine);
        }
        

        static void ProcessVehicleParking<T>(List<T> parking, VehicleType vehicleType) where T : Vehicle
        {
            Console.WriteLine($"Which action do you want to perform for {vehicleType}? (P for Parking / U for Unparking)");
            char action = char.Parse(Console.ReadLine().ToUpper());
            Console.WriteLine("Enter the vehicle index:");
            int index = int.Parse(Console.ReadLine());
            index--;
            if (index < 0 || index >= parking.Count)
            {
                throw new IndexOutOfRangeException("Invalid vehicle index.");
            }
            switch (action)
            {
                case 'P':
                    string ticket = TicketGenerator.GenerateTicket();
                    Console.WriteLine("Enter the vehicle number:");
                    int vehicleNumber = int.Parse(Console.ReadLine());
                    Console.WriteLine("parking place");
                    string location = Console.ReadLine();
                    DateTime currentTime = DateTime.Now;
                    string inTime = currentTime.ToString("hh:mm tt");
                    parking[index].ParkVehicle(ticket, vehicleNumber, location, inTime, "");
                    break;
                case 'U':
                    DateTime currentOutTime = DateTime.Now;
                    string outTime = currentOutTime.ToString("hh:mm tt");
                    parking[index].UnparkVehicle(outTime);
                    break;
                default:
                    throw new InvalidOperationException("Invalid action.");
            }
        }
    }

    }
    abstract class Vehicle
    {
        public int VehicleNumber { get; set; }
        public string Ticket { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string Location { get; set; }
        public virtual void ParkVehicle(string ticket, int vehicleNumber, string location, string inTime, string? outtime)
        {
            Ticket = ticket;
            VehicleNumber = vehicleNumber;
            Location = location;
            // OutTime = OutTime;
            InTime = inTime;


            Console.WriteLine($"You have parked your vehicle. Location: {location},Ticket No:{Ticket}, In Time: {inTime}");

            // Console.WriteLine($"You have Unparked your vehicle. intime: {InTime}, out Time: {OutTime}");
        }
        public virtual void UnparkVehicle(string outTime)
        {
            OutTime = outTime;
            Console.WriteLine($"Vehicle {VehicleNumber} unparked, Out Time: {outTime}");

        }

        public virtual string GetParkingStatus()
        {
            if (VehicleNumber == 0)
            {
                return "Spot is empty.";
            }
            else
            {
                return $"vehicle parked ," +
                    $" location.{Location},Ticket No: {Ticket}, Vehicle No: {VehicleNumber}, In Time: {InTime}";
            }
        }


        public virtual string GetUnParkingStatus()
        {
            if (VehicleNumber == 0)
            {
                return "Spot is empty.";
            }
            else
            {
                return $"vehicle parked ," +
                    $" location.{Location},Ticket No: {Ticket}, Vehicle No: {VehicleNumber}, In Time: {InTime},Out Time: {OutTime}";
            }
        }

    }
    class TwoWheel : Vehicle
    {
    }
    class FourWheel : Vehicle
    {
    }
    class HeavyVehicle : Vehicle
    {
    }




