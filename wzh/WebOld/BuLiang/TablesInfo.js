var COM_COLS = {
    guid: {
        style: {
            width: '100px'
        }
    },
    email: {
        style: {
            width: '300px'
        },
        ipt_rule: [
            {
            rule: /^$|^[^\s@]+@[^\s@]+\.[^\s@]+$/,
            msg: "メールアドレスNG"
            }
        ]

    }
}


var DB_TABLES = {
    m_user: {




    }
}

function GetChildsProperty(inProperty, defaultValue) {
    let p;
    let arr = inProperty.split('.');

    if (COM_COLS.hasOwnProperty(arr[0])) {
        p = COM_COLS[arr[0]];
        if (arr.length == 1) return p;
    } else {
        return defaultValue;
    }
    for (let i = 1 ; i <= arr.length - 1; i++) {
        if (p.hasOwnProperty(arr[i])) {
            p = p[arr[i]];
            if (i == arr.length - 1) return p;
        } else {
            return defaultValue;
        }
    }
    return defaultValue;
}