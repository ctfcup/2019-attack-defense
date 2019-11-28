var router = require('express').Router();

router.get('/', async function(req, resp, next) {
    const res = await req.app.get('orm')
                        .tables.users.filter({
                            id: req.authedUserId
                        });
    
    if (res.length == 0)
        resp.sendStatus(404);
    const user = res[0];
    const planners = await req.app.get('orm')
                        .tables.planners.filter({
                            owner: req.authedUserId
                        });
    resp.render('planners.html', {isAuthed: true, planners: planners});
});

router.post('/', async function(req, resp, next) {
    const res = await req.app.get('orm')
                        .tables.users.filter({
                            id: req.authedUserId
                        });
    if (res.length == 0)
        resp.sendStatus(404);
    const user = res[0];
    let id = await req.app.get('orm')
                    .tables.planners.add({
                        owner: user.id, 
                        name: req.body.name, 
                        description: req.body.description,
                        creationTime: req.body.creationTime
                    });
    resp.json('Planner created')
});

router.get('/:planner_id', async function(req, resp, next) {
    if (req.body.start_week === undefined) {
        c_date = new Date();
        c_date.setDate(c_date.getDate() - c_date.getDay() + 1);
        start_week = String(c_date.getFullYear()).padStart(2, '0') + '-' 
                        + String(c_date.getMonth()+1).padStart(2, '0') + '-'
                        +String(c_date.getDate()).padStart(2, '0');
    } else start_week = req.body.start_week;
    end_date = new Date(start_week);
    end_date.setDate(end_date.getDate() + 7);
    end_week = String(end_date.getFullYear()).padStart(2, '0') + '-' 
                        + String(end_date.getMonth()+1).padStart(2, '0') + '-'
                        +String(end_date.getDate()).padStart(2, '0');
    
    let planner = await req.app.get('orm')
                        .tables.planners.filter({
                            id: parseInt(req.params.planner_id), owner: req.authedUserId
                        });
    if (planner.length == 0)
        resp.sendStatus(404);
    planner = planner[0];
    console.log(`--==Planner ${planner.id}==--`)
    let tasks = await req.app.get('orm')
                        .tables.tasks.filter({
                            planner: planner.id
                        });
    console.log(tasks.filter(t => start_week<=t.date && t.date<=end_week).map(t=>{t.date = new Date(t.date).getDay(); return t;}));
    tasks = tasks.filter(t => start_week<=t.date && t.date<=end_week).map(t => {t.date=new Date(t.date).getDay();return t;})
    console.log(tasks);
    
    let hours = [];
    for (let hour=6; hour < 23; hour+=2)
        hours.push(`${String(hour).padStart(2,"0")}:00`);
    let week = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
    
    resp.render('planner.html', {isAuthed: true, planner_name: planner.name, hours: hours, 
                                    week_days: week});
});

router.post('/:planner_id', async function(req, resp, next) {
    const planner = await req.app.get('orm')
                        .tables.planners.filter({
                            id: parseInt(req.params.planner_id), owner: req.authedUserId
                        });
    if (planner.length == 0)
        resp.sendStatus(404);
    console.log(req.body);
    
    let id = await req.app.get('orm')
                    .tables.tasks.add({
                        planner: planner[0].id, 
                        name: req.body.name, 
                        description: req.body.description,
                        date: req.body.date,
                        time: req.body.time
                    });
    resp.json("Added")
});


module.exports = router;