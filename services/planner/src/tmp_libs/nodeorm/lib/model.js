'use strict';

class Model {
    #db = '';
    #types = {
        'string': 'TEXT',
        'number': 'INT'
    };
    #table_name = '';
    #fields = {id: 'number'};
    
    isCorrectField(name, value) {
        return name in this.#fields && typeof(value) == this.#fields[name];
    }
    
    prechecking(str) { return str.replace(/[<>\\();/]/g, '') }
    
    getTableName() {
        return this.#table_name;
    }
    
    constructor(table_name, fields) {
        this.#table_name = table_name;
        
        for (let field in fields) {
            let t = typeof(fields[field]());
            if (t in this.#types)
                this.#fields[field] = t;
            else
                throw new Error('Incorrect data type');
        }
    }
    
    makeCorrectValues(params) {
        for (let key in params)
            if (typeof(params[key]) == 'string')
                params[key] = `"${this.checking(this.prechecking(params[key])).trim()}"`;
        return params;
    }
    
    async createTable(db) {
        this.#db = db;
        let fieldsStr = 'id SERIAL';
        let valuesStr = '';
        for (let key in this.#fields)
            if (key != 'id')
                fieldsStr += `,${key} ${this.#types[this.#fields[key]]}`;
        return await this.#db.query(`CREATE TABLE IF NOT EXISTS ${this.#table_name} (${fieldsStr});`);
    }
    
    async add(params) {
        params = this.makeCorrectValues(params);
        
        let p_fields = '';
        let p_values = '';
        Object.entries(params).forEach(function(el) {
            if (this.isCorrectField(el[0], el[1])) {
                p_fields += `${el[0]},`;
                p_values += `${el[1]},`;
            }
        }, this);
        let answ = await this.#db.query(`INSERT INTO ${this.#table_name} (${p_fields.slice(0,-1)}) VALUES (${p_values.slice(0,-1)});`);
        return answ[0].insertId;
    }
    
    async filter(params) {
        params = this.makeCorrectValues(params);
        
        let filter_string = '';
        Object.entries(params).forEach(function(el) {
            if (this.isCorrectField(el[0], el[1])) {
                filter_string += `${el[0]}=${el[1]} AND `;
            }
        }, this);
        if (filter_string)
            filter_string = ` WHERE ${filter_string.slice(0,-5)}`;
        let answ = await this.#db.query(`SELECT * FROM ${this.#table_name}${filter_string};`);
        return answ[0];
    }
    
    async remove(params) {
        params = this.makeCorrectValues(params);
        
        let filter_string = '';
        Object.entries(params).forEach(function(el) {
            if (this.isCorrectField(el[0], el[1])) {
                filter_string += `${el[0]}=${el[1]} AND `;
            }
        }, this);
            
        if (filter_string)
            filter_string = ` WHERE ${filter_string.slice(0,-5)}`;
        let answ = await this.#db.query(`DELETE FROM ${this.#table_name}${filter_string};`);
        return answ[0].affectedRows;
    }

    checking(str) { return str.replace(/[^\w@+-:=. ]/, '') }
    
    async update(filter_params, params) {
        filter_params = this.makeCorrectValues(filter_params);
        params = this.makeCorrectValues(params);
        
        let filter_string = '';
        Object.entries(filter_params).forEach(function(el) {
            if (this.isCorrectField(el[0], el[1]))
                filter_string += `${el[0]}=${el[1]} AND `;
        }, this);

        if (filter_string)
            filter_string = `WHERE ${filter_string.slice(0,-5)}`;
        
        let set_string = '';
        Object.entries(params).forEach(function(el) {
            if (this.isCorrectField(el[0], el[1]))
                set_string += `${el[0]}=${el[1]},`;
        }, this);
        
        let answ = await this.#db.query(`UPDATE ${this.#table_name} SET ${set_string.slice(0,-1)} ${filter_string};`);
        return answ[0].affectedRows;
    }
}

module.exports = Model;