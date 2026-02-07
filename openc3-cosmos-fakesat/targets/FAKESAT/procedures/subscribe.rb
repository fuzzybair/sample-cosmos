id = subscribe_packets([['FAKESAT', 'HEALTH_STATUS'], ['FAKESAT', 'IMAGE']])
cmd("FAKESAT COLLECT with TYPE NORMAL, DURATION 0")
wait 1
cmd("FAKESAT COLLECT with TYPE NORMAL, DURATION 0")
wait 1
cmd("FAKESAT COLLECT with TYPE NORMAL, DURATION 0")
wait 1

id, packets = get_packets(id)
puts "First batch:"
packets.each do |packet|
  puts "#{packet['PACKET_TIMESECONDS']}: #{packet['target_name']} #{packet['packet_name']}"
end
# Reuse ID from last call, allow for 1s wait, only get 1 packet
puts "Second batch:"
id, packets = get_packets(id, block: 1000, count: 1)
packets.each do |packet|
  puts "#{packet['PACKET_TIMESECONDS']}: #{packet['target_name']} #{packet['packet_name']}"
end
