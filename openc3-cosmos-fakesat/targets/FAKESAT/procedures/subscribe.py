id = subscribe_packets([["FAKESAT", "HEALTH_STATUS"], ["FAKESAT", "IMAGE"]])
cmd("FAKESAT COLLECT with TYPE NORMAL, DURATION 0")
wait(1)
cmd("FAKESAT COLLECT with TYPE NORMAL, DURATION 0")
wait(1)
cmd("FAKESAT COLLECT with TYPE NORMAL, DURATION 0")
wait(1)

id, packets = get_packets(id)
print("First batch:")
for packet in packets:
    print(
        f"{packet['PACKET_TIMESECONDS']}: {packet['target_name']} {packet['packet_name']}"
    )
# Reuse ID from last call, allow for 1s wait, only get 1 packet
print("Second batch:")
id, packets = get_packets(id, block=1000, count=1)
for packet in packets:
    print(
        f"{packet['PACKET_TIMESECONDS']}: {packet['target_name']} {packet['packet_name']}"
    )
