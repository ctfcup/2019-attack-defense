'use strict';
const mysql = require('mysql2/promise');

class DB {
    #pool = '';
    tables = {};
    
    end () {
        this.#pool.end();
    }
    
    setConfig(params) {
        if (!(params.host && params.user && params.password && params.database && params.tables))
            throw new Error('[-] Some params for connection to db not found');
        this.#pool = mysql.createPool({
            host: params.host,
            user: params.user,
            password: params.password,
            database: params.database,
            connectionLimit: 25
        });
        
        params.tables.forEach(function(t) {
            this.tables[t.getTableName()] = t;
        }, this)
        const db = this;
        let res = Promise.all(Object.values(db.tables).map(t => t.createTable(db)));
        res.then((result) => { return result; }).catch( error => { return error; });
    }
    
    async query(q){
        let res = '';
        await this.#pool.getConnection()
            .then(conn => {
                const r = conn.query(q);
                conn.release();
                return r;
            })
            .then(result => {
                res = result;
            }).catch(error => {
                console.log(`[-] ${error}`);
                res = error;
            });
        return res;        
    }
}

module.exports = new DB();
