require 'openc3/interfaces/http_client_interface'
require 'json'

module OpenC3
  class MyJsonInterface < HttpClientInterface
    def convert_packet_to_data(packet)
      # We just turn the packet into JSON. 
      # We don't need to set variables here anymore.
      packet.as_json.to_json
    end

    def write_interface(packet_data)
      # 1. Grab settings from plugin.txt or defaults
      method = (@options['HTTP_METHOD'] || 'POST').to_s.upcase
      path   = (@options['HTTP_PATH']   || '/').to_s
      
      Logger.info "!!! MyJsonInterface: Sending JSON POST to #{path} !!!"

      # 2. BYPASS super(packet_data) to avoid the "Unsupported Method" crash.
      # We use the parent's internal @client directly.
      # send_request(method, path, body, headers, query_params)
      response = @client.send_request(method, path, packet_data, { 'Content-Type' => 'application/json' }, {})
      
      # 3. Check for errors (Optional but recommended)
      if response.code.to_i >= 300
        Logger.error "!!! MyJsonInterface: Remote Server returned #{response.code}: #{response.body} !!!"
      end
    rescue => e
      Logger.error "!!! MyJsonInterface Error: #{e.message} !!!"
      raise e
    end
  end
end