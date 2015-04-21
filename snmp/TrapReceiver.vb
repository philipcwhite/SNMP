Imports System.Threading
Imports System.Net.Sockets
Imports System.IO
Imports System.Net

Public Class TrapReceiver


    Shared port As Integer = 162
    Shared client As New UdpClient(port)
    Shared receivePoint As New IPEndPoint(IPAddress.Any, port)
    Shared SNMP_OID_Val_Pair_List As New List(Of SNMP_Val_Pairs)

    'Change up to add to bytelist and subtract off used items?  Recursive loop through oids?

    Public Shared Sub WaitForPackets()
        While True

            Dim PacketData As Byte() = client.Receive(receivePoint)
            Dim PacketLength As Integer = PacketData.Length
            Dim PacketList As New List(Of Byte)
            PacketList.AddRange(PacketData)
            Dim SNMP_HostAddress As String = receivePoint.Address.ToString
            Dim SNMPVersion As String = Nothing
            Dim CommunityString As String = Nothing
            Dim RequestID As String = Nothing
            Dim Packet_SNMP_ID As Integer = Nothing
            Dim Packet_Item_Length As Integer = Nothing
            Dim Packet_Item_Bytes As Byte() = Nothing
            Dim Remaining_Packet As String = Nothing
            Dim SNMP_OID_Val_Pair As String = Nothing
            Dim ByteList As New List(Of Byte)



            If PacketLength < 255 Then

                PacketList.RemoveRange(0, 4)
                Packet_SNMP_ID = PacketList.Item(0)
                SNMPVersion = Packet_SNMP_ID + 1
                PacketList.RemoveRange(0, 2)


                Packet_Item_Length = PacketList.Item(0)
                For i = 1 To Packet_Item_Length
                    ByteList.Add(PacketList.Item(i))
                Next i
                Packet_Item_Bytes = ByteList.ToArray
                ByteList.Clear()
                CommunityString = System.Text.Encoding.ASCII.GetString(Packet_Item_Bytes)
                PacketList.RemoveRange(0, Packet_Item_Length + 4)


                Packet_Item_Length = PacketList.Item(0)
                For i = 1 To Packet_Item_Length
                    ByteList.Add(PacketList.Item(i))
                Next i
                Packet_Item_Bytes = ByteList.ToArray
                ByteList.Clear()

                RequestID = BitConverter.ToString(Packet_Item_Bytes)
                RequestID = RequestID.Replace("-", "")
                RequestID = Convert.ToInt64(RequestID, 16)
                PacketList.RemoveRange(0, Packet_Item_Length + 1)

                'Do something with Error data (6 bytes)
                'Remove Error Data
                PacketList.RemoveRange(0, 6)

                'Remove sequence start and length
                PacketList.RemoveRange(0, 2)

                'add in oid vs value code

                While PacketList.Count > 0

                    Packet_Item_Length = Nothing
                    Packet_Item_Bytes = Nothing
                    SNMP_OID_Val_Pair = Nothing

                    If PacketList.Item(0) = 48 Then
                        PacketList.RemoveRange(0, 1)
                        Packet_Item_Length = PacketList.Item(0)
                    End If

                    For i = 1 To Packet_Item_Length
                        ByteList.Add(PacketList.Item(i))
                    Next i

                    DecodeValPairs(ByteList)

                    PacketList.RemoveRange(0, Packet_Item_Length + 1)

                End While

            End If

            Console.WriteLine("Receiveing Packet...")
            Console.WriteLine("Host Address=" & SNMP_HostAddress)
            Console.WriteLine("Packet Length=" & PacketLength.ToString)
            Console.WriteLine("SNMP Version=" & SNMPVersion)
            Console.WriteLine("Community String=" & CommunityString)
            Console.WriteLine("Request ID=" & RequestID)

            For Each i In SNMP_OID_Val_Pair_List
                Console.WriteLine("SNMP OID=" & i.SNMP_OID & " SNMP Value=" & i.SNMP_Value)
            Next

            Console.WriteLine(vbCrLf)

            SNMP_OID_Val_Pair_List.Clear()

        End While

    End Sub

    Public Shared Sub DecodeValPairs(ByVal ByteList As List(Of Byte))
        Dim SNMP_OID As String = Nothing
        Dim SNMP_Value As String = Nothing

        'Add SNMP OID Number
        If ByteList.Item(0) = 6 Then
            For i = 2 To ByteList.Item(1) + 1
                SNMP_OID = SNMP_OID & ByteList.Item(i) & "."
            Next

            SNMP_OID = SNMP_OID.Substring(0, SNMP_OID.Length - 1)
            SNMP_OID = SNMP_OID.Replace("43.", "1.3.")

            ByteList.RemoveRange(0, ByteList.Item(1) + 2)


        End If

        'Add SNMP OID Values
        If ByteList.Item(0) = 67 Then

            'For i = 2 To ByteList.Item(1) + 1
            '    SNMP_Value = SNMP_Value & ByteList.Item(i) & ","
            'Next

            Dim SNMP_Time_Bytes As Byte() = Nothing

            SNMP_Time_Bytes = ByteList.GetRange(2, ByteList.Item(1)).ToArray
            SNMP_Value = BitConverter.ToString(SNMP_Time_Bytes)
            SNMP_Value = SNMP_Value.Replace("-", "")
            SNMP_Value = Convert.ToInt64(SNMP_Value, 16) & " Milliseconds"

            'SNMP_Value = SNMP_Value.Substring(0, SNMP_Value.Length - 1)

            ByteList.RemoveRange(0, ByteList.Item(1) + 2)
        ElseIf ByteList.Item(0) = 6 Then
            For i = 2 To ByteList.Item(1) + 1
                SNMP_Value = SNMP_Value & ByteList.Item(i) & "."
            Next

            SNMP_Value = SNMP_Value.Substring(0, SNMP_Value.Length - 1)
            SNMP_Value = SNMP_Value.Replace("43.", "1.3.")

            ByteList.RemoveRange(0, ByteList.Item(1) + 2)
        End If

        SNMP_OID_Val_Pair_List.Add(New SNMP_Val_Pairs With {.SNMP_OID = SNMP_OID, .SNMP_Value = SNMP_Value})



    End Sub

    Private Class SNMP_Val_Pairs
        Public Property SNMP_OID As String
        Public Property SNMP_Value As String
    End Class

End Class
