FROM ruby:3.2-slim

ARG USERNAME=vscode
ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update \
  && apt-get install -y --no-install-recommends \
    build-essential \
    git \
    curl \
    ca-certificates \
    libssl-dev \
    libreadline-dev \
    zlib1g-dev \
  && rm -rf /var/lib/apt/lists/*

RUN gem install bundler --no-document

# Create non-root user for VS Code remote (if needed)
RUN useradd -m ${USERNAME} || true

WORKDIR /workspace

CMD ["bash"]

#docker run --rm -it -v "`pwd`:/workspace" -w /workspace fakesat-gem-builder bash