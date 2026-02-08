require 'openc3/interfaces/http_client_interface'

module OpenC3
  class MyHttpClientInterface < HttpClientInterface
    def write(packet)
      # 1. Safely handle HTTP_PATH
      begin
        # This will CRASH if the item is missing, jumping us to the rescue block
        packet.get_item('HTTP_PATH')
      rescue RuntimeError
        # It crashed, so we know it's missing. Define it as a virtual item (length 0).
        packet.define_item('HTTP_PATH', 0, 0, :STRING)
      end
      # Now we can safely write the value
      packet.write('HTTP_PATH', '/api/command')

      # 2. Safely handle HTTP_METHOD
      begin
        packet.get_item('HTTP_METHOD')
      rescue RuntimeError
        packet.define_item('HTTP_METHOD', 0, 0, :STRING)
      end
      packet.write('HTTP_METHOD', 'POST')

      Logger.error "!!! MyHttpClientInterface: Patching complete. Sending #{packet.packet_name}... !!!"
      
      # 3. Call super. 
      # The parent class will now find 'HTTP_PATH' and 'HTTP_METHOD' in the packet 
      # and proceed to send the request without crashing.
      super(packet)
    rescue => e
      Logger.error "!!! MyHttpClientInterface: CRASHED: #{e.message} !!!"
      raise e
    end
  end
end