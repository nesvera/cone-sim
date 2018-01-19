import socket
import sys
import csv
import keyboard

# Get recording file
file_path = "18.01.18_02.32.txt"
file = open(file_path, "r")

# Break recording file
telemetry = []
for row in csv.reader(file, delimiter = ';'):
    telemetry.append(row)


## ate aqui e soh pra brincar

# Set udp communication
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

receive_address = ('127.0.0.1', 5000)
send_address = ('127.0.0.1', 5001 )

print('Receiving on ' + str(receive_address) + ' port.')
print('Sending on ' + str(send_address) + ' port.\n')

sock.bind(receive_address)

i = 0

while True:
    
    try:
    	if keyboard.is_pressed('q'):
    		break

    	#print('Wainting to receive message')
    	data, address = sock.recvfrom(4096)

    	#print(data)

    	if data:
    	
    		if i <= len(telemetry)-1:
    			msg = telemetry[i][1] + ";" + telemetry[i][2] + ";" + telemetry[i][3] + ";" + telemetry[i][4]
    			#print msg

    			sent = sock.sendto(msg, send_address)

    			i += 1
    		
    		else:
    			print "caiu caralho"
    			sock.close()
    			break

    		pass

    finally:
    	pass
