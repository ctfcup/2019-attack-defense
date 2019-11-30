'use strict';
const orm = require('node-fluent-orm');
const Model = orm.Model;


module.exports = new Model('users', {
    login: String,
    password: String,
    email: String,
    firstName: String,
    secondName: String,
    phoneNumber: String,
    additionalInfo: String
});
