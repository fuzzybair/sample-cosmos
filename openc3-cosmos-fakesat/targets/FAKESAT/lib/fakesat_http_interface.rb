require 'openc3/interfaces/http_client_interface'

module OpenC3
  # Class name MUST be the CamelCase version of the filename
  class FakesatHttpInterface < HttpClientInterface
    def initialize(host, port, protocol, write_timeout, read_timeout, connect_timeout)
      # 1. Log that we are starting (This will show up in Operator logs)
      Logger.info "FakesatHttpInterface: Initializing for target #{host}"
      
      # 2. Fix the "nil" error by creating the queue manually
      @request_queue = Thread::Queue.new
      
      # 3. Call parent
      super(host, port, protocol, write_timeout, read_timeout, connect_timeout)
    end
  end
end