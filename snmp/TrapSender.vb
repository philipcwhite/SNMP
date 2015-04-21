Imports System.Threading
Imports System.Net.Sockets
Imports System.IO
Imports System.Net

Public Class TrapSender

    Public Shared Sub Send_Trap()
        Dim client As New UdpClient
     
        Dim data As Byte() = {48, 92, 2, 1, 1, 4, 6, 112, 117, 98, 108, 105, 99, 167, 79, 2, 4, 90, 73, 169, 195, 2, 1, 0, 2, 1, 0, 48, 65, 48, 16, 6, 8, 43, 6, 1, 2, 1, 1, 3, 0, 67, 4, 9, 230, 61, 38, 48, 23, 6, 10, 43, 6, 1, 6, 3, 1, 1, 4, 1, 0, 6, 9, 43, 6, 1, 6, 3, 1, 1, 5, 5, 48, 20, 6, 10, 43, 6, 1, 6, 3, 1, 1, 4, 3, 0, 6, 6, 43, 6, 1, 2, 1, 11}
        Console.WriteLine("Sending packet...")

        client.Send(data, data.Length, "localhost", 162)
        Console.WriteLine("Packet sent")

    End Sub

End Class
