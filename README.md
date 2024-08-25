# Receipt Processor Challenge

This project is a .NET 8.0 Web API that processes receipt data and calculates points based on predefined rules. The application is containerized using Docker, making it easy to deploy and run.

## Prerequisites

- **Docker**: Make sure Docker is installed and running on your machine.
  - [Docker Desktop for macOS and Windows](https://www.docker.com/products/docker-desktop/)
  - [Docker Engine for Linux](https://docs.docker.com/engine/install/)

- **Git**: Ensure Git is installed to clone the repository.
  - [Git Installation Guide](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)

## Getting Started

### 1. Clone the Repository

First, clone the repository to your local machine using the following command:

```bash
git clone https://github.com/LewisJor/receipt-processor-challenge.git
cd receipt-processor-challenge/SimpleReceiptProcessor
```

### 2. Build the Docker Image

Navigate to the `SimpleReceiptProcessor` directory and build the Docker image:

```bash
docker build -t simplereceiptprocessor .
```

This command will build the Docker image and tag it as `simplereceiptprocessor`.

### 3. Run the Docker Container

After building the image, run a container from the image:

```bash
docker run -d -p 8080:8080 -p 8081:8081 simplereceiptprocessor
```
- View Swagger at `http://localhost:8080/swagger/index.html`

### 4. Testing the API

Once the container is running, you can test the API using `curl` commands.

#### 4.1. Test the `POST /receipts/process` Endpoint

Use the following `curl` command to submit a receipt for processing:

```bash
curl -X POST http://localhost:8080/receipts/process \
-H "Content-Type: application/json" \
-d '{
  "retailer": "Target",
  "purchaseDate": "2022-01-01",
  "purchaseTime": "13:01",
  "items": [
    {
      "shortDescription": "Mountain Dew 12PK",
      "price": "6.49"
    },{
      "shortDescription": "Emils Cheese Pizza",
      "price": "12.25"
    },{
      "shortDescription": "Knorr Creamy Chicken",
      "price": "1.26"
    },{
      "shortDescription": "Doritos Nacho Cheese",
      "price": "3.35"
    },{
      "shortDescription": "   Klarbrunn 12-PK 12 FL OZ  ",
      "price": "12.00"
    }
  ],
  "total": "35.35"
}'
```

This command sends a `POST` request to the `/receipts/process` endpoint with the receipt data in JSON format. If successful, the API will return an ID for the processed receipt.

#### 4.2. Test the `GET /receipts/{id}/points` Endpoint

After processing a receipt, you can retrieve the points associated with it using the following `curl` command. Replace `{id}` with the actual ID returned from the previous POST request:

```bash
curl -X GET http://localhost:8080/receipts/e43f7f1c-e6b0-444e-95f1-115be2237a4c/points
```

This command sends a `GET` request to the `/receipts/{id}/points` endpoint and returns the points for the specified receipt.

## Stopping the Docker Container

To stop the running container, use the following command:

```bash
docker stop $(docker ps -q --filter ancestor=simplereceiptprocessor)
```

This command stops any running containers based on the `simplereceiptprocessor` image.

---