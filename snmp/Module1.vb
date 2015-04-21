Imports snmp_send_receive_ca.TrapReceiver
Imports snmp_send_receive_ca.TrapSender
Imports System.Threading


Module Module1

    Sub Main()

        'Listen for Traps
        Dim t As Thread
        t = New Thread(AddressOf WaitForPackets)
        t.Start()

        'Send Trap
        Send_Trap()

    End Sub

End Module
