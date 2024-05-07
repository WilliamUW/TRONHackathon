//SPDX-License-Identifier: MIT
pragma solidity >=0.8.0 <0.9.0;

/**
 * A smart contract that allows the storing of 3D objects with their name, description, owner, and model URL on the TRON blockchain.
 * @author William Wang
 */
contract TRON3DObjects {
    uint256 private _tokenIdCounter = 0;

    // State Variables
    string public greeting = "3D Objects on the Blockchain!";
    bool public premium = false;
    uint256 public totalCounter = 0;
    mapping(address => uint) public userGreetingCounter;

    struct Object {
        string name;
        string description;
        string modelUrl;
        address owner;
    }

    Object[] public objects;

    function insertObject(
        string memory _name,
        string memory _description,
        string memory _modelUrl,
        address nftOwner
    ) public {
        require(bytes(_name).length > 0, "Name cannot be empty");
        require(bytes(_description).length > 0, "Description cannot be empty");
        require(bytes(_modelUrl).length > 0, "_modelUrl cannot be empty");

        objects.push(Object(_name, _description, _modelUrl, nftOwner));
    }

    function deleteObject(uint256 index) public {
        require(index < objects.length, "Invalid index");
        delete objects[index];
    }

    function readObjects() public view returns (Object[] memory) {
        return objects;
    }

    constructor() {
    }
}
